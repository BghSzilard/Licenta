namespace AutoCorrectorEngine;

public class Settings
{
    public static string LLMRunningLocation { get; set; } = "Local";
    public static string ZipPath { get; set; } = "";
    public static string ProjectsPath { get; set; } = "";
    public static string SolutionPath { get; set; }
    public static string ScriptsPath { get; set; }
    public static string TranslateScriptPath { get; set; }
    public static string UnitTestScriptPath { get; set; }
    public static string ResultsPath  { get; set; }
    public static string UnzippedFolderPath { get; set; }
    static Settings()
    {
        SolutionPath = Directory.GetCurrentDirectory();
        for (int i = 0; i < 4; ++i)
        {
            SolutionPath = Directory.GetParent(SolutionPath)!.FullName;
        }
        ResultsPath = Path.Combine(SolutionPath, "Results");
        UnzippedFolderPath = Path.Combine(SolutionPath, "Unzipped");
        ScriptsPath = Path.Combine(SolutionPath, "Scripts");
        TranslateScriptPath = Path.Combine(ScriptsPath, "Translate.py");
        UnitTestScriptPath = Path.Combine(ScriptsPath, "UnitTester.py");
    }
}