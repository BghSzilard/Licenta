using SharpCompress.Common;

namespace AutoCorrectorEngine;

public class CorrectionChecker
{
    public async Task<string> CheckMethodCorrectness(string function, string requirement, string functionName)
    {
        LLMManager lLMManager = new LLMManager();
        var result = await lLMManager.DetermineCorrectness(requirement, function);
        return result;

    }

    public async Task<string> CheckSourceFileCorrectness(string sourceFile, string requirement)
    {
        string fileContent;

        using (StreamReader sr = new StreamReader(sourceFile))
        {
            fileContent = sr.ReadToEnd();
        }

        LLMManager lLMManager = new LLMManager();
        var result = await lLMManager.DetermineCorrectnessSourceFile(requirement, fileContent);
        return result;

    }

    public async Task<string> MakeUnitTests(string processedReq, string function, string functionName)
    {
        processedReq = processedReq[12..];
        processedReq = processedReq.Insert(0, "\"");
        processedReq += "\"";
        processedReq = processedReq.Replace(".", "");
        processedReq = processedReq.Replace("\n", "");
        functionName = functionName.Insert(0, "\"");
        functionName += "\"";

        var tempFile = Path.Combine(Settings.SolutionPath, "temp.h");

        File.WriteAllText(tempFile, function);

        tempFile = tempFile.Insert(0, "\"");
        tempFile += "\"";
        var arguemnts = $"python '{Settings.UnitTestScriptPath}' '{processedReq}' '{functionName}' '{tempFile}'";
        ProcessExecutor processExecutor = new ProcessExecutor();
        string result = await processExecutor.ExecuteProcess("powershell", arguemnts, "");
        result = result.Replace("\n", "");
        result = result.Replace("\r", "");

        return result;
    }
}