using System.Text;
using System.Text.RegularExpressions;
using AutoCorrectorEngine;

class Program
{
    static string FindIncludes(string filePath)
    {
        StringBuilder includesBuilder = new StringBuilder();
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Match match = Regex.Match(line, @"^\s*#include\s*[<""](.*)[>""]\s*$");
                if (match.Success)
                {
                    includesBuilder.AppendLine(line);
                }
            }
        }
        return includesBuilder.ToString();
    }
    public static async Task Main()
    {
        string requirement = "Scrieți o funcție care ia ca argument un vector de int și returnează suma elementelor";
        //string subrequirement = "Check for the following input output pairs: (12 65 3 5 1 2) -> (1 2 3 5 12 65); (6 5 4 9) -> (4 5 6 9)";
        LLMManager lLMManager = new LLMManager();
        string functionSignature = "\"";
        functionSignature += lLMManager.GetFunctionSignature(requirement);
        functionSignature += "\"";

        string pathTestFile = "C:\\Users\\z004w26z\\Desktop\\Test.cpp";
        FunctionSignatureExtractorWrapper functionSignatureExtractor = new FunctionSignatureExtractorWrapper();
        var signatures = functionSignatureExtractor.GetSignatures(pathTestFile);
        string allSignatures = "";
        allSignatures += "\"";
        foreach (var signature in signatures)
        {
            allSignatures += signature;
            allSignatures += ";";
        }
        allSignatures = allSignatures.Remove(allSignatures.Length - 1);
        allSignatures += "\"";
        string pathMatchFinder = "C:\\Users\\z004w26z\\Desktop\\matchFinder.py ";
        ProcessExecutor executor = new ProcessExecutor();
        var functionMatch = executor.ExecuteProcess("python", pathMatchFinder + allSignatures + " " + functionSignature, "");
        functionMatch = functionMatch.Replace("\n", "");
        functionMatch = functionMatch.Replace("\r", "");
        FunctionExtractorWrapper functionExtractor = new FunctionExtractorWrapper();
        var includes = FindIncludes(pathTestFile);
        var function = includes;
        function += functionExtractor.GetFunction(pathTestFile, functionMatch);
        Console.WriteLine(function);
    }
}