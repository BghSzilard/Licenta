namespace AutoCorrectorEngine;

public class LLMManager
{
    private string RunModel(string modelName, string requirement)
    {
        Translate translate = new Translate();
        string requirementEnglish = translate.TranslateToEnglish(requirement);

        string apiLocation = "/api/generate";

        if (Settings.LLMRunningLocation == "Local")
        {
            apiLocation = apiLocation.Insert(0, "http://localhost:11434");
        }
        else
        {
            apiLocation = apiLocation.Insert(0, Settings.LLMRunningLocation);
        }

        string script = $@"
        $response = Invoke-RestMethod -Method Post -Uri '{apiLocation}' -ContentType 'application/json' -Body (@{{
            model = '{modelName}'
            prompt = '{requirementEnglish}'
        }} | ConvertTo-Json)

        $response";

        ProcessExecutor processExecutor = new ProcessExecutor();
        string result = processExecutor.ExecuteProcess("powershell.exe", "-Command \"& {" + script + "}\"", "");


        var lines = result.Trim().Split('\n');
        var concatenatedResponse = string.Concat(lines.Select(line =>
        {
            var startIndex = line.IndexOf("\"response\":\"") + "\"response\":\"".Length;
            var endIndex = line.IndexOf("\",", startIndex);
            var response = line.Substring(startIndex, endIndex - startIndex);
            return response.Replace("\\u003e", ">").Replace("\\\"", "\"");
        }));

        return concatenatedResponse;
    }
    public string GetFunctionSignature(string requirement)
    {
        return RunModel("extractor", requirement);
    }

    public string ProcessSubtask(string subtask)
    {
        var fileContent = RunModel("moddec3", subtask);
        return fileContent;
    }
    public string RequirementCorrectionDecider(string requirement, string functionName, string headerPath)
    {
        var fileContent = RunModel("m4", requirement);

        if (fileContent.Contains("Check correctness"))
        {
            fileContent = fileContent[18..];
            fileContent = fileContent.Insert(0, "\"");
            fileContent += "\"";
            fileContent = fileContent.Replace(".", "");
            fileContent = fileContent.Replace("\n", "");
            functionName = functionName.Insert(0, "\"");
            functionName += "\"";
            headerPath = headerPath.Insert(0, "\"");
            headerPath += "\"";
            var arguemnts = fileContent + " " + functionName + " " + headerPath;
            ProcessExecutor processExecutor = new ProcessExecutor();
            fileContent = processExecutor.ExecuteProcess("python", "C:\\Users\\z004w26z\\Desktop\\UnitTester.py " + arguemnts, "");
        }

        return fileContent;
    }
}