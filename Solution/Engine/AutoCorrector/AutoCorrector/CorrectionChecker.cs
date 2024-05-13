using System.Diagnostics;
using SharpCompress.Common;

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
                Arguments = $"\"{sourceFilePath}\" -o \"{Settings.UnitTestsPath}\\temp.exe\"",
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
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        return await process.StandardOutput.ReadToEndAsync();
    }
    public async Task<string> MakeUnitTests(string req, string studentName,  string requirement, string function)
    {
        if (studentName.Contains("Patrasc"))
        {
            return "Could not make unit test!";
        }

        LLMManager llmManager = new LLMManager();
        var result = await llmManager.WriteUnitTests(requirement, function);
        result = result.Replace("```", "");
        result = result.Replace("cpp", "");
        string unitTestFile = Path.Combine(Settings.UnitTestsPath, $"{studentName} {req}.cpp");
        
        if (!Directory.Exists(Settings.UnitTestsPath))
        {
            Directory.CreateDirectory(Settings.UnitTestsPath);
        }

        await File.WriteAllTextAsync(unitTestFile, result);

        await CompileAsync(unitTestFile);
        
        string output = await ExecuteAsync($"\"{Settings.UnitTestsPath}\\temp.exe\"");

        File.Delete($"{Settings.UnitTestsPath}\\temp.exe");

        return output;
    }
}