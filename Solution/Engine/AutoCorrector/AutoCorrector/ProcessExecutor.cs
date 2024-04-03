using System.Diagnostics;
using System.Text;

namespace AutoCorrectorEngine;

public class ProcessExecutor
{
    public async Task<string> ExecuteProcess(string command, string arguments, string input)
    {
        arguments = arguments.Replace("\r", "");
        arguments = arguments.Replace("\n", "");
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = $"{command}";
        startInfo.Arguments = $"\"{arguments}\" ";
        startInfo.EnvironmentVariables["OLLAMA_HOST"] = "https://41d8-35-229-215-119.ngrok-free.app";


        if (input != "") 
        {
            if (startInfo.FileName.Contains("powershell"))
            {
                startInfo.Arguments += $"'{input}'";
                startInfo.EnvironmentVariables["OLLAMA_HOST"] = "https://41d8-35-229-215-119.ngrok-free.app";
            }
            else
            {
                startInfo.Arguments += $"\"{input}\"";

            }
        }

        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.CreateNoWindow = true;

        process.StartInfo = startInfo;

        // StringBuilder to store the output and error messages
        StringBuilder outputBuilder = new StringBuilder();
        StringBuilder errorBuilder = new StringBuilder();

        // Event handlers to capture output and error messages
        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                outputBuilder.AppendLine(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                errorBuilder.AppendLine(e.Data);
            }
        };


        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();


        // Wait for the process to exit asynchronously
        await Task.Run(() =>
        {
            process.WaitForExit();
        });

        var result = outputBuilder.ToString();
        result = result.Replace("failed to get console mode for stdout: The handle is invalid.\r\nfailed to get console mode for stderr: The handle is invalid.\r\n", "");

        return result;
    }
}
