using Newtonsoft.Json.Linq;

namespace AutoCorrectorEngine;

public class LLMManager
{
    private async Task<string> RunModel(string modelName, string requirement)
    {
        string endpoint = "https://api.together.xyz/v1/chat/completions";
        JObject baseRequestBody = JObject.Parse(@"
    	{
        	""model"": ""meta-llama/Llama-3-70b-chat-hf"",
        	""max_tokens"": 1000,
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
            client.DefaultRequestHeaders.Add("Authorization", "Bearer 05d1354737d556199fd699294221fa327d70519941d67ab98f53285058e81d96");

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

            if (modelName != "unitTester")
            {
                contentValue = contentValue.Replace("\r", "");
                contentValue = contentValue.Replace("\n", "");
                contentValue = contentValue.Replace("  ", "");
            }

            return contentValue;
        }
    }

    private JObject GetModel(string modelName, JObject body)
    {
        JObject systemPrompt = new JObject();
        systemPrompt["role"] = "system";

        switch (modelName)
        {
            case "extractor":
                systemPrompt["content"] = "You are an expert in C++ function evaluation. You have only one task. You are given a list of C++ function signatures and a requirement. What you have to do is to answer with the name of the only function that is solving the task. Don't write anything else in your answer, just the name of the function. For example, if the name of the function is 'isPrime', your answer will consist of 'isPrime' and that word only. If no function is close to solving the requirement, answer with 'None'";
                break;
            case "engine3":
                systemPrompt["content"] = "You are a useful assistant in analysing requirements regarding C++ functions. Your job is to answer with one of 3 words depending on the nature of the requirement. If the requirement is about checking time complexity, your answer will be 'timeComplexity' followed by the timeComplexity in question. If the requirement is about checking the correctness of a function, your answer will be 'correctness'. If the requirement is about the presence of a container or a function, your write 'presence' followed by the function / container the presence of which we care about. Remember, your answer will consist only of the keyword and the following words that the requirement specifies. Do not write anything else. For example, if the requirement is: 'Check the correctness of the function by checking the following input-output pairs: 12 -> 25; 9 -> 43', your answer will be 'correctness'. If the requirement is 'Check if the time complexity of the function is O(n)', your answer will be 'complexity O(n)'. If the requirement is 'Check if the function std::sort is used', you answer with 'presence std::sort'.\r\n";
                break;
            case "autoCorrecter":
                systemPrompt["content"] = "You are given a requirement regarding a C++ function and the C++ function that is trying to respect the requirement. Your job is to determine whether or not the function respects the whole requirement (you only consider that the function respects the requirement if it fully respects it, make your evaluation rigorous). If it respects it, you write \"Yes: \" and start writing the reason why it respects it, otherwise start your answer with \"No: \" and write the reason the function doesn't / doesn't fully respect the requirement. Your answer mustn't consist of more than 100 tokens";
                break;
            case "sourceFileCorrecter":
                systemPrompt["content"] = "You are given a requirement regarding a C++ source file and the C++ source file. Your job is to determine whether or not the source file respects the whole requirement (you only consider that the source file respects the requirement if it fully respects it, make your evaluation rigorous). If it respects it, you write \"Yes: \" and start writing the reason why it respects it, otherwise start your answer with \"No: \" and write the reason the function doesn't / doesn't fully respect the requirement. Your answer mustn't consist of more than 100 tokens";
                break;
            case "unitTester":
                systemPrompt["content"] = "You are given a C++ function. Write an auxiliary function to check if for a given input the output is the expected one. In case the output matches the expected output, you should print on screen: \"All unit tests passed\", ohterwise \"Input {givenInput} Expected output {expectedOutput} Real Output {realOutput}. In your answer, include the given function. Do not change the given function. In your answer, provide only the code, without saying anything.";
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
}