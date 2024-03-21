namespace AutoCorrectorEngine;

public class LLMManager
{
    public string GetFunctionSignature(string requirement)
    {
        Translate translate = new Translate();
        string requirementEnglish = translate.TranslateToEnglish(requirement);
        ProcessExecutor processExecutor = new ProcessExecutor();
        var functionSignature = processExecutor.ExecuteProcess("ollama", "run extractor", requirementEnglish);
        functionSignature = functionSignature.Replace("\n", "");
        return functionSignature;
    }

    public string RequirementCorrectionDecider(string requirement)
    {
        ProcessExecutor processExecutor = new ProcessExecutor();
        Translate translate = new Translate();
        string requirementEnglish = translate.TranslateToEnglish(requirement);
        var answer = processExecutor.ExecuteProcess("ollama", "run m3", requirementEnglish);
        if (answer.Contains("Check correctness"))
        {
            answer = answer[18..];
            answer = answer.Replace(".", "");
            answer = answer.Replace("\n", "");
            string argument = "\"'1 7 6' '1 7 6' - '1 3 4' '1 3 4'\"";
            answer = processExecutor.ExecuteProcess("python", "C:\\Users\\z004w26z\\Desktop\\UnitTester.py " + argument, "");
        }

        return answer;
    }
}