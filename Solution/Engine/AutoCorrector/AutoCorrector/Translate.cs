namespace AutoCorrectorEngine;

public class Translate
{
    public async Task<string> TranslateToEnglish(string text)
    {
        ProcessExecutor processExecutor = new ProcessExecutor();
        return await processExecutor.ExecuteProcess("python", Settings.TranslateScriptPath, text);
    }
}