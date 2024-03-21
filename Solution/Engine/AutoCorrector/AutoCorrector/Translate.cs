using System.Diagnostics;

namespace AutoCorrectorEngine;

public class Translate
{
    public string TranslateToEnglish(string text)
    {
        ProcessExecutor processExecutor = new ProcessExecutor();
        return processExecutor.ExecuteProcess("python", "C:\\Users\\z004w26z\\Desktop\\Translate.py", text);
    }
}