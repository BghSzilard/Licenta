namespace AutoCorrectorEngine;

public class LLMManager
{
    private async Task<string> RunModel(string modelName, string requirement)
    {
        //Translate translate = new Translate();
        //string requirementEnglish = await translate.TranslateToEnglish(requirement);

        //string apiLocation = "/api/generate";

        //if (Settings.LLMRunningLocation == "Local")
        //{
        //    apiLocation = apiLocation.Insert(0, "http://localhost:11434");
        //}
        //else
        //{
        //    apiLocation = apiLocation.Insert(0, Settings.LLMRunningLocation);
        //}

        //string script = $@"
        //$response = Invoke-RestMethod -Method Post -Uri '{apiLocation}' -ContentType 'application/json' -Body (@{{
        //    model = '{modelName}'
        //    prompt = '{requirementEnglish}'
        //}} | ConvertTo-Json)

        //$response";

        //ProcessExecutor processExecutor = new ProcessExecutor();
        //string result = await processExecutor.ExecuteProcess("powershell.exe", "-Command \"& {" + script + "}\"", "");


        //var lines = result.Trim().Split('\n');
        //var concatenatedResponse = string.Concat(lines.Select(line =>
        //{
        //    var startIndex = line.IndexOf("\"response\":\"") + "\"response\":\"".Length;
        //    var endIndex = line.IndexOf("\",", startIndex);
        //    var response = line.Substring(startIndex, endIndex - startIndex);
        //    return response.Replace("\\u003e", ">").Replace("\\\"", "\"");
        //}));

        //return concatenatedResponse;

        Translate translate = new Translate();
        requirement = await translate.TranslateToEnglish(requirement);

        ProcessExecutor processExecutor = new ProcessExecutor();
        string command = $"ollama run {modelName} '{requirement}'";
        return await processExecutor.ExecuteProcess("powershell", command, "");
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
            ProcessExecutor processExecutor = new ProcessExecutor();
            var input = await processExecutor.ExecuteProcess("powershell", $"ollama run unitTester '{subtask}'", "");
            return input;
        }

        return fileContent;
    }
}