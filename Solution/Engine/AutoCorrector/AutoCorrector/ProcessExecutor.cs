using System.Diagnostics;

namespace AutoCorrectorEngine;

public class ProcessExecutor
{
    public string ExecuteProcess(string command, string arguments, string input)
    {
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

            if (input != "")
            {
                process.StandardInput.WriteLine(input);
            }

            process.StandardInput.Flush();
            process.StandardInput.Close();

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();

            process.WaitForExit();

            return stdout;
        }
    }
}