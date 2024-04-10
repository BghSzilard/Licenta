using System.Diagnostics;
using AutoCorrector;

namespace AutoCorrectorEngine;

public class CorrectionChecker
{
    public async Task<bool> CheckCorrectness(string fileContent, string functionName, string headerPath, Requirement requirement)
    {
        if (fileContent.Contains("correctness"))
        {
            string result = await MakeUnitTests(fileContent, functionName, headerPath);
            requirement.SubRequirements.Add(new SubRequirement() { Title = result.Replace("Success!", "").Replace("Fail!", "")}) ;

            if(result.Contains("Success!"))
            {
                return true;
            }

            return false;
        }

        return true;
    }
    public async Task<string> MakeUnitTests(string fileContent, string functionName, string headerPath)
    {
        fileContent = fileContent[12..];
        fileContent = fileContent.Insert(0, "\"");
        fileContent += "\"";
        fileContent = fileContent.Replace(".", "");
        fileContent = fileContent.Replace("\n", "");
        functionName = functionName.Insert(0, "\"");
        functionName += "\"";
        headerPath = headerPath.Insert(0, "\"");
        headerPath += "\"";
        var arguemnts = $"python '{Settings.UnitTestScriptPath}' '{fileContent}' '{functionName}' '{headerPath}'";
        ProcessExecutor processExecutor = new ProcessExecutor();
        string result = await processExecutor.ExecuteProcess("powershell", arguemnts, "");
        result = result.Replace("\n", "");
        result = result.Replace("\r", "");

        return result;
    }
}