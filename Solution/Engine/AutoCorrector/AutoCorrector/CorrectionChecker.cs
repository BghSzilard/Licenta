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

    static async Task CompileAsync(string sourceFilePath, string studentName)
    {
        var process = new Process
        {
            StartInfo =
        {
            FileName = "powershell.exe",
            Arguments = $"g++ -o '{Settings.UnitTestsPath}\\{studentName}.exe' '{sourceFilePath}'",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true, // Redirect standard error
            CreateNoWindow = true,
        }
        };

        var output = "";
        var error = "";

        process.Start();

        // Read the standard output and error asynchronously.
        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        // Wait for both reads to complete.
        await Task.WhenAll(outputTask, errorTask);

        // Assign the results to the variables.
        output = outputTask.Result;
        error = errorTask.Result;

        // Here you can use the output and error variables as needed.

        // Ensure the process has exited before returning.
        await process.WaitForExitAsync();
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

        await CompileAsync(unitTestFile, studentName);

        var filePath = $"{Settings.UnitTestsPath}\\{studentName}.exe";

        if (!File.Exists(filePath))
        {
            return "Could not make unit test!";
        }

        string output = await ExecuteAsync($"\"{Settings.UnitTestsPath}\\{studentName}.exe\"");

        //File.Delete($"{Settings.UnitTestsPath}\\{studentName}.exe");

        return output;
    }
}