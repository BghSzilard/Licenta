using AutoCorrector;
using System;
using System.Diagnostics;
using System.Text;
class Program
{
    static void Translate()
    {
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python";
        start.Arguments = "C:\\Users\\z004w26z\\Desktop\\Translate.py";
        start.UseShellExecute = false;
        start.RedirectStandardInput = true;
        start.RedirectStandardOutput = true;

        using (Process process = Process.Start(start))
        {
            if (process != null)
            {
                // Send input to the Python script
                string text = "Bonjour le monde!"; // Example non-English text
                process.StandardInput.WriteLine(text);

                // Read output from the Python script
                string output = process.StandardOutput.ReadToEnd();
                Console.WriteLine("Translation result: " + output);
            }
            else
            {
                Console.WriteLine("Failed to start Python process.");
            }
        }
    }
    static string GetParamsAndReturnType(string question)
    {
        string command = "ollama";
        string arguments = "run extractor";

        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process())
        {
            process.StartInfo = processStartInfo;
            process.Start();

            process.StandardInput.WriteLine(question);
            process.StandardInput.Flush();
            process.StandardInput.Close();

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();

            process.WaitForExit();

            return stdout;
        }
    }
    public static async Task Main()
    {
        //StudentManager studentManager = new StudentManager();
        //await studentManager.Solve();
        //Console.WriteLine(GetParamsAndReturnType("Write a function that takes as argument a vector of ints and a float and returns the elements sum"));
        Translate();
    }
}