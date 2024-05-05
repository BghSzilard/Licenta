using System.Diagnostics;

namespace AutoCorrectorEngine;

public class CorrectionChecker
{
    public async Task<string> CheckMethodCorrectness(string function, string requirement)
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

    static Task CompileAsync(string sourceFilePath)
    {
        var process = new Process
        {
            StartInfo =
            {
                FileName = "clang++",
                Arguments = $"{sourceFilePath} -o temp.exe",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true,
            }
        };

        process.Start();
        return process.WaitForExitAsync();
    }

    static async Task<string> ExecuteAsync(string fileName)
    {
        var process = new Process
        {
            StartInfo =
            {
                FileName = fileName,
                UseShellExecute = false,
                RedirectStandardOutput = true
            }
        };

        process.Start();
        return await process.StandardOutput.ReadToEndAsync();
    }
    public async Task<string> MakeUnitTests(string requirement, string function)
    {
        LLMManager llmManager = new LLMManager();
        var result = await llmManager.WriteUnitTests(requirement, function);
        result = result.Replace("```", "");
        result = result.Replace("cpp", "");
        string tempFile = "myTempFile.cpp";
        string path = Path.Combine(Settings.SolutionPath, tempFile);

        await File.WriteAllTextAsync(path, result);

        await CompileAsync(path);
        string output = await ExecuteAsync("temp.exe");

        File.Delete(path);
        File.Delete("temp.exe");

        return output;
    }
    //public async Task<string> MakeUnitTests(string processedReq, string function, string functionName)
    //{
    //    processedReq = processedReq[12..];
    //    processedReq = processedReq.Insert(0, "\"");
    //    processedReq += "\"";
    //    processedReq = processedReq.Replace(".", "");
    //    processedReq = processedReq.Replace("\n", "");
    //    functionName = functionName.Insert(0, "\"");
    //    functionName += "\"";

    //    var tempFile = Path.Combine(Settings.SolutionPath, "temp.h");

    //    File.WriteAllText(tempFile, function);

    //    tempFile = tempFile.Insert(0, "\"");
    //    tempFile += "\"";
    //    var arguemnts = $"python '{Settings.UnitTestScriptPath}' '{processedReq}' '{functionName}' '{tempFile}'";
    //    ProcessExecutor processExecutor = new ProcessExecutor();
    //    string result = await processExecutor.ExecuteProcess("powershell", arguemnts, "");
    //    result = result.Replace("\n", "");
    //    result = result.Replace("\r", "");

    //    return result;
    //}
}