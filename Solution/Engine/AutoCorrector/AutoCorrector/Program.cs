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
        DateTime startTime = DateTime.Now;
        //string requirement = "Scrieti o functie care primeste un int si determina daca numarul este prim sau nu";
        //string subrequirement = "Sa se efectueze verificari pentru urmatoarele perechi de input output: 12 -> false; 11 -> true; 50 -> true";
        string requirement = "Scrieti o functie care primeste un int n si determina numarul fibonacci corespunzator";
        string subrequirement = "Sa se efectueze verificari pentru urmatoarele perechi de input output: 4 -> 3; 5 -> 9";

        LLMManager lLMManager = new LLMManager();
        string functionSignature = "\"";


        functionSignature += lLMManager.GetFunctionSignature(requirement);

        functionSignature = functionSignature.Replace("\n", "");
        functionSignature = functionSignature.Replace("\r", "");
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
        var arguments = pathMatchFinder + allSignatures + " " + functionSignature;
        ProcessExecutor executor = new ProcessExecutor();
        var functionMatch = executor.ExecuteProcess("python", arguments, "");
        functionMatch = functionMatch.Replace("\n", "");
        functionMatch = functionMatch.Replace("\r", "");
        FunctionExtractorWrapper functionExtractor = new FunctionExtractorWrapper();
        var includes = FindIncludes(pathTestFile);
        var function = includes;
        function += functionExtractor.GetFunction(pathTestFile, functionMatch);

        DateTime endTime = DateTime.Now;
        TimeSpan elapsedTime = endTime - startTime;

        DateTime newTime = DateTime.Now;

        // Get the total number of seconds passed
        double secondsPassed = elapsedTime.TotalSeconds;

        Console.WriteLine("Seconds passed: " + secondsPassed);

        Console.WriteLine(function);

        string extractedFilePath = "C:\\Users\\z004w26z\\Desktop\\extractedTest.h";
        File.WriteAllText(extractedFilePath, function);


        Console.WriteLine(lLMManager.RequirementCorrectionDecider(subrequirement, functionMatch, extractedFilePath));
        DateTime endTime2 = DateTime.Now;
        TimeSpan elapsed2 = endTime2 - newTime;
        secondsPassed = elapsed2.TotalSeconds;
        Console.WriteLine("Seconds passed: " + secondsPassed);


    }
}