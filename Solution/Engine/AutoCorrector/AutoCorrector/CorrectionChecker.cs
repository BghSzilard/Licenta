using System.Diagnostics;
using AutoCorrector;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AutoCorrectorEngine;

public class CorrectionChecker
{
    public async Task<string> CheckCorrectness(string function, string requirement, string functionName)
    {
        if (requirement.Contains("correctness"))
        {
            return await MakeUnitTests(requirement, function, functionName);
        }
        else
        {
            LLMManager lLMManager = new LLMManager();
            var result = await lLMManager.DetermineCorrectness(requirement, function);
            return result;
        }
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