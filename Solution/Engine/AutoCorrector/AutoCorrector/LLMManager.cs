using Newtonsoft.Json.Linq;

namespace AutoCorrectorEngine;

public class LLMManager
{
    private async Task<string> RunModel(string modelName, string requirement)
    {
        Translate translate = new Translate();
        requirement = await translate.TranslateToEnglish(requirement);   

        string endpoint = "https://api.together.xyz/v1/chat/completions";
        JObject baseRequestBody = JObject.Parse(@"
    	{
        	""model"": ""codellama/CodeLlama-34b-Instruct-hf"",
        	""max_tokens"": 512,
        	""temperature"": 0,
        	""top_p"": 0,
        	""top_k"": 0,
        	""repetition_penalty"": 1,
        	""stop"": [
            	""<step>""
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

            // Read response content
            string responseContent = await response.Content.ReadAsStringAsync();

            // Parse JSON
            JObject jsonResponse = JObject.Parse(responseContent);

            // Access the value of the "content" attribute
            string contentValue = jsonResponse["choices"][0]["message"]["content"].ToString();

            contentValue = contentValue.Replace("\r", "");
            contentValue = contentValue.Replace("\n", "");
            contentValue = contentValue.Replace("  ", "");

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
                systemPrompt["content"] = "You are an expert C++ programmer. You have only one job, and you answer only in one format.\r\nYour job is the following: you are given a requirement regarding the implementation of a C++ function and\r\nyou have to extract the parameter types in the following format: {parameter1}, {parameter2}, ... {parametern}\r\nFor example, if the user's input is: 'Write a function that takes as parameter an int and a float and raises\r\nthe float to the third power', you are to answer: '{int, float} -> float'\r\nIf the user mentions a number regarding one of the parameter types, you are to write that parameter down\r\nas many times, as the requirement says. For example, 'Write a function that takes as parameter two strings\r\nand three floats', you would answer: '{string, string, float, float, float}'. If any clue given about the\r\nreturn type, include it in your answer by writing the '->' and after that write the retunr type. Remember, you are not allowed to say anything else in your answer than the format, regardless of what the prompt is!";
                break;
            case "engine3":
                systemPrompt["content"] = "You are a useful assistant in analysing requirements regarding C++ functions. Your job is to answer with one of 3 words depending on the nature of the requirement. If the requirement is about checking time complexity, your answer will be 'timeComplexity' followed by the timeComplexity in question. If the requirement is about checking the correctness of a function, your answer will be 'correctness'. If the requirement is about the presence of a container or a function, your write 'presence' followed by the function / container the presence of which we care about. Remember, your answer will consist only of the keyword and the following words that the requirement specifies. Do not write anything else. For example, if the requirement is: 'Check the correctness of the function by checking the following input-output pairs: 12 -> 25; 9 -> 43', your answer will be 'correctness'. If the requirement is 'Check if the time complexity of the function is O(n)', your answer will be 'complexity O(n)'. If the requirement is 'Check if the function std::sort is used', you answer with 'presence std::sort'.\r\n";
                break;
            case "unitTester":
                systemPrompt["content"] = "You are a useful assistant in analysing requirements regarding C++ functions. Your job is to write in the same format the prompt of the user, namely: you write in dollar marks the input value of the first input-output pair, in dollar marks the output of the first input-output pair, you write a '-' sign to represent the delimitation from the following input-output pair and so on. Do not write anything else. For example, if the requirement is: 'Check the correctness of the function by checking the following input-output pairs: 12 -> 25; 9 -> 43', your answer will be: 'correctness $12$ $25$ - $9$ $43$' (Do not forget to start your answer with the word 'correctness', to use the dollar marks for both the input and the output (regardless of whether they are numbers or strings) and the '-' sign. Keep in mind, regardless of how many pairs there are, you use the same syntax for all of them, using dollar signs, where the first pair of dollars delimits the input, the second pair delimits the output and the '-' sign delimits a pair of input-output).";
                    break;
        }

        ((JArray)body["messages"]).Add(systemPrompt);


        return body;
    }

    public async Task<string> GetFunctionSignature(string requirement)
    {
        return await RunModel("extractor", requirement);
    }

    public async Task<string> ProcessSubtask(string subtask)
    {
        var fileContent = await RunModel("engine3", subtask);

        Translate translate = new Translate();
        subtask = await translate.TranslateToEnglish(subtask);

        if (fileContent.Contains("correctness"))
        {
            var input = await RunModel("unitTester", subtask);
            return input;
        }

        return fileContent;
    }
}