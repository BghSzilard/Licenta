using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

namespace AutoCorrectorEngine;

public class LLMManager
{
    private async Task<string> RunModel(string modelName, string requirement)
    {
        if (Settings.LLMRunningLocation == "Server")
        {
            string endpoint = "https://api.together.xyz/v1/chat/completions";
            JObject baseRequestBody = JObject.Parse(@"
    	    {
        	    ""model"": ""meta-llama/Llama-3-70b-chat-hf"",
        	    ""max_tokens"": 2000,
        	    ""temperature"": 0,
        	    ""top_p"": 0,
        	    ""top_k"": 1,
        	    ""repetition_penalty"": 1,
        	    ""stop"": [
            	    ""<|eot_id|>""
        	    ],
        	    ""messages"": []
    	    }");

            baseRequestBody = GetModel(modelName, baseRequestBody);

            JObject newMessage = new JObject();
            newMessage["content"] = $"{requirement}";
            newMessage["role"] = "user";

            ((JArray)baseRequestBody["messages"]).Add(newMessage);

            using (HttpClient client = new HttpClient())
            {
                // Add authorization header
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Settings.APIKey}");

                // Create the request content
                HttpContent content = new StringContent(baseRequestBody.ToString());

                // Set content type
                content.Headers.ContentType.MediaType = "application/json";

                // Send POST request
                HttpResponseMessage response = await client.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    return await RunModel(modelName, requirement);
                }

                // Read response content
                string responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    return await RunModel(modelName, requirement);
                }

                // Parse JSON
                JObject jsonResponse = JObject.Parse(responseContent);

                // Access the value of the "content" attribute
                string contentValue = jsonResponse["choices"][0]["message"]["content"].ToString();

                if (modelName != "unitTester" && modelName != "scaleCreator")
                {
                    contentValue = contentValue.Replace("\r", "");
                    contentValue = contentValue.Replace("\n", "");
                    contentValue = contentValue.Replace("  ", "");
                }

                return contentValue;
            }
        }
        else
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "powershell.exe",
                    Arguments = $"ollama run '{modelName}' '{requirement}'",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true, // Redirect standard error
                    CreateNoWindow = true,
                }
            };

            var output = "";
            var error = "";

            process.Start();

            // Read the standard output and error asynchronously.
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            // Wait for both reads to complete.
            await Task.WhenAll(outputTask, errorTask);

            // Assign the results to the variables.
            output = outputTask.Result;
            error = errorTask.Result;

            // Here you can use the output and error variables as needed.

            // Ensure the process has exited before returning.
            await process.WaitForExitAsync();

            return output;
        }
    }

    private JObject GetModel(string modelName, JObject body)
    {
        JObject systemPrompt = new JObject();
        systemPrompt["role"] = "system";

        switch (modelName)
        {
            case "extractor":
                systemPrompt["content"] = "You are an expert in C++ function evaluation. You have only one task. You are given a list of C++ function signatures and a requirement. What you have to do is to answer with the name of the only function that is solving the task. Don't write anything else in your answer, just the name of the function. For example, if the name of the function is 'isPrime', your answer will consist of 'isPrime' and that word only. If no function is close to solving the requirement, answer with 'None'. If the name of the function is given in the requirement, but among the functions signatures given, there is no function with such name, answer with 'None'";
                break;
            case "autoCorrecter":
                systemPrompt["content"] = "You are given a requirement regarding a C++ function and the C++ function that is trying to respect the requirement. Your job is to determine whether or not the function respects the whole requirement (you only consider that the function respects the requirement if it fully respects it, make your evaluation rigorous). If it respects it, you write \"Yes: \" and start writing the reason why it respects it, otherwise start your answer with \"No: \" and write the reason the function doesn't / doesn't fully respect the requirement. If the function is not provided, answer with: 'No: The function is not provided'. Your answer mustn't consist of more than 100 tokens";
                break;
            case "sourceFileCorrecter":
                systemPrompt["content"] = "You are given a requirement regarding a C++ source file and the C++ source file. Your job is to determine whether or not the source file respects the whole requirement (you only consider that the source file respects the requirement if it fully respects it, make your evaluation rigorous). If it respects it, you write \"Yes: \" and start writing the reason why it respects it, otherwise start your answer with \"No: \" and write the reason the function doesn't / doesn't fully respect the requirement. Your answer mustn't consist of more than 100 tokens";
                break;
            case "unitTester":
                systemPrompt["content"] = "You are given a task regarding creating a unit test for a provided C++ function. Your job is to write a unit test to determine if the function produces the expected output for given inputs. Follow these instructions carefully:\r\n\r\n    Preserve the Function: Include the provided function exactly as it is. Do not change or correct the function, regardless of any errors it may contain. The purpose of the unit test is to check the correctness of the provided function as-is.\r\n\r\n    Unit Test Function: Write an auxiliary function to check if the actual output matches the expected output for given inputs. If the output matches the expected output, print \"All unit tests passed\"; otherwise, print \"Input {givenInput} Expected output {expectedOutput} Real Output {realOutput}\".\r\n\r\n    Output Only Code: Provide only the C++ code in your response, ensuring that it is complete and can be compiled without additional comments or explanations.\r\n\r\nHere is an example user input and the expected response format:\r\n\r\nExample User Input:\r\n\r\nc\r\n\r\nDetermine if the function is implemented correctly by checking if for the input {12, 55, 44} the output is {12, 44, 55}\r\n#include <iostream>\r\n#include <vector>\r\n\r\n// Bubble Sort Function\r\nvoid bubbleSort(std::vector<int>& arr) {\r\n    int n = arr.size();\r\n    bool swapped;\r\n    for (int i = 0; i < n-1; i++) {\r\n        swapped = false;\r\n        for (int j = 0; j < n-i-1; j++) {\r\n            if (arr[j] > arr[j+1]) {\r\n                std::swap(arr[j], arr[j+1]);\r\n                swapped = true;\r\n            }\r\n        }\r\n        // If no two elements were swapped by inner loop, then break\r\n        if (!swapped)\r\n            break;\r\n    }\r\n}\r\n\r\nExpected Response:\r\n\r\ncpp\r\n\r\n#include <iostream>\r\n#include <vector>\r\n\r\n// Bubble Sort Function\r\nvoid bubbleSort(std::vector<int>& arr) {\r\n    int n = arr.size();\r\n    bool swapped;\r\n    for (int i = 0; n-1; i++) {\r\n        swapped = false;\r\n        for (int j = 0; j < n-i-1; j++) {\r\n            if (arr[j] > arr[j+1]) {\r\n                std::swap(arr[j], arr[j+1]);\r\n                swapped = true;\r\n            }\r\n        }\r\n        // If no two elements were swapped by inner loop, then break\r\n        if (!swapped)\r\n            break;\r\n    }\r\n}\r\n\r\n// Function to print the array in a formatted way for comparison\r\nstd::string arrayToString(const std::vector<int>& arr) {\r\n    std::string result = \"[\";\r\n    for (int i = 0; i < arr.size(); i++) {\r\n        result += std::to_string(arr[i]);\r\n        if (i < arr.size() - 1) {\r\n            result += \", \";\r\n        }\r\n    }\r\n    result += \"]\";\r\n    return result;\r\n}\r\n\r\n// Unit Test Function\r\nvoid unitTestBubbleSort() {\r\n    // Test input\r\n    std::vector<int> arr = {12, 55, 44};\r\n    \r\n    // Expected output\r\n    std::vector<int> expectedArr = {12, 44, 55};\r\n    \r\n    // Perform bubble sort on the input array\r\n    bubbleSort(arr);\r\n    \r\n    // Check if the sorted array matches the expected output\r\n    bool testPassed = (arr == expectedArr);\r\n    if (testPassed) {\r\n        std::cout << \"All unit tests passed!\" << std::endl;\r\n    } else {\r\n        std::cout << \"Input: \" << arrayToString({12, 55, 44}) << \", \"\r\n                  << \"Expected output: \" << arrayToString(expectedArr) << \", \"\r\n                  << \"Real output: \" << arrayToString(arr) << std::endl;\r\n    }\r\n}\r\n\r\nint main() {\r\n    // Call the unit test function\r\n    unitTestBubbleSort();\r\n    return 0;\r\n}";
                break;
            case "scaleCreator":
                systemPrompt["content"] = "You are a useful assistant in creating xml scales from natural language. Your job is to use the following structure and tags for creating xml scales: first you write the tasks tag, which encompasses all tasks. After that, for each task, you use the task tag. Inside the task tag, use the title tag, which will contain the description of the task. Also inside the task tag, use the points tag, where you store the number of points for the given task. Also inside the task tag, use the type tag, the value of which can be either \"method\" or \"source file\", depending on whether the task is referring to a certain method or the whole source file. Inside the task tag, you also write the subtasks tag, which will contain all the subtasks which belong to the current task. For a certain subtask, you use the tag subtask, where you store the its description inside the title tag. Similarly, use the points tag to reflect the number of points given for the current subtask. If the subtasks specifies that it is to do a unit test, you use the tag type, where you write \"unitTest\", otherwise, omit this tag. Here is an example input:\r\n1. Write a method called minPathLength that for a matrix that has n rows and m columns and for k given positions, determines the minimum length path that starts from position i1 and j1 and passes through all k positions (doesn't matter in which order), finally arriving at position i2 and j2 (2.5 points)\r\n1.1 Determine with the following input and output if the function is correctly implemented. (For the path, consider each step in any direction as value 1, not the value of the cell) \r\n\r\n       Input:\r\n      Matrix:\r\n\r\n      1 2 3\r\n      4 5 6\r\n      7 8 9\r\n\r\n      Positions: (1, 1), (1, 3), (3, 3)\r\n      Starting position: (1, 1)\r\n      Ending position: (3, 3)\r\n      Expected output: 4\r\n\r\n(0.5 points)\r\n\r\n1.2: Determine if the time complexity is at most O(n * m * k), where n and m are the dimensions of the matrix and k is the number of given positions. (0.5 points)\r\n1.3: Determine if the space complexity is at most O(n * m) (0.5 points)\r\n1.4: Determine if the function handles exceptions (0.5 points)\r\n2: Determine if the code respects clean code rules (1 point) (source file requirement)\r\n2.1: Determine if the code respects clean code rules no magic numbers, avoid having more than 3 parameters for a function (1 point) \r\n3: Implement a function that sorts efficiently in descending order a vector of integers (2 points)\r\n3.1: Determine if the sorting is done correctly by checking if for the input: 12, 25, 9, 77 the output is 77, 25, 12, 9 (1 point, unit test)\r\n3.2: Determine if the time complexity is O(n log(n)) (1 point)\r\n\r\nFor this input, your answer should be:\r\n<tasks>\r\n  <task>\r\n    <title>Write a method called minPathLength that for a matrix that has n rows and m columns and for k given positions, determines the minimum length path that starts from position i1 and j1 and passes through all k positions (doesn't matter in which order), finally arriving at position i2 and j2.</title>\r\n    <points>2.5</points>\r\n    <type>method</type>\r\n    <subtasks>\r\n      <subtask>\r\n        <title>\r\n      Determine with the following input and output if the function is correctly implemented. (For the path, consider each step in any direction as value 1, not the value of the cell) \r\n\r\n       Input:\r\n      Matrix:\r\n\r\n      1 2 3\r\n      4 5 6\r\n      7 8 9\r\n\r\n      Positions: (1, 1), (1, 3), (3, 3)\r\n      Starting position: (1, 1)\r\n      Ending position: (3, 3)\r\n      Expected output: 4\r\n      </title>\r\n        <points>0.5</points>\r\n      </subtask>\r\n      <subtask>\r\n        <title>Determine if the time complexity is at most O(n * m * k), where n and m are the dimensions of the matrix and k is the number of given positions.</title>\r\n        <points>0.5</points>\r\n      </subtask>\r\n      <subtask>\r\n        <title>Determine if the space complexity is at most O(n * m)</title>\r\n        <points>0.5</points>\r\n      </subtask>\r\n      <subtask>\r\n        <title>Determine if the function handles exceptions</title>\r\n        <points>0.5</points>\r\n      </subtask>\r\n    </subtasks>\r\n  </task>\r\n  <task>\r\n    <title>Determine if the code respects clean code rules</title>\r\n    <points>1</points>\r\n    <subtasks>\r\n    <subtask>\r\n      <title>Determine if the code respects clean code rules no magic numbers, avoid having more than 3 parameters for a function</title>\r\n      <points>1</points>\r\n    </subtask>\r\n    </subtasks>\r\n    <type>source file</type>\r\n  </task>\r\n  <task>\r\n    <title>Implement a function that sorts efficiently in descending order a vector of integers</title>\r\n    <points>2</points>\r\n    <type>method</type>\r\n    <subtasks>\r\n      <subtask>\r\n        <title>Determine if the sorting is done correctly by checking if for the input: 12, 25, 9, 77 the output is 77, 25, 12, 9</title>\r\n        <points>1</points>\r\n        <type>unitTest</type>\r\n      </subtask>\r\n      <subtask>\r\n        <title>Determine if the time complexity is O(n log(n))</title>\r\n        <points>1</points>\r\n      </subtask>\r\n    </subtasks>\r\n  </task>\r\n</tasks>\r\n\r\nIn your answer, write the xml and the xml only";
                break;
        }

        ((JArray)body["messages"]).Add(systemPrompt);


        return body;
    }

    public async Task<string> WriteUnitTests(string requirement, string function)
    {
        return await RunModel("unitTester", $"\"{requirement}\" \n {function}");
    }
    public async Task<string> GetFunctionName(string requirement)
    {
        return await RunModel("extractor", requirement);
    }

    public async Task<string> DetermineCorrectness(string requirement, string function)
    {
        return await RunModel("autoCorrecter", $"\"{requirement}\" \n {function}");
    }

    public async Task<string> DetermineCorrectnessSourceFile(string requirement, string function)
    {
        return await RunModel("sourceFileCorrecter", $"\"{requirement}\" \n {function}");
    }
    public async Task<string> ProcessSubtask(string subtask)
    {
        var fileContent = await RunModel("unitTester", subtask);

        if (fileContent.StartsWith("No"))
        {
            return subtask;
        }
        
        return fileContent;
    }

    public async Task<string> CreateScale(string doc)
    {
        var scale = await RunModel("scaleCreator", doc);
        return scale;
    }
}