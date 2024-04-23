using AutoCorrector;

namespace AutoCorrectorEngine;

public class Settings
{
    public static string LLMRunningLocation { get; set; } = "Local";
    public static string ZipPath { get; set; } = "";
    public static string ProjectsPath { get; set; } = "";
    public static string SolutionPath { get; set; }
    public static string ScriptsPath { get; set; }
    public static string ModelFilesPath { get; set; }
    public static string ExtractorPath { get; set; }
    public static string ModDecPath { get; set; }
    public static string ConverterPath { get; set; }
    public static string TranslateScriptPath { get; set; }
    public static string UnitTestScriptPath { get; set; }
    public static string MatchFinderScriptPath { get; set; }
    public static string ResultsPath  { get; set; }
    public static string UnzippedFolderPath { get; set; }
    public static int PlagiarismThreshold {  get; set; }
    public static StudentInfo StudentSample {  get; set; }
    public static List<StudentInfo> Students { get; set; }
    public static string PlagiarismResFolder {  get; set; }
    public static List<PlagiarismPair> PlagiarismPairs { get; set; }
    static Settings()
    {
        SolutionPath = Directory.GetCurrentDirectory();
        for (int i = 0; i < 4; ++i)
        {
            SolutionPath = Directory.GetParent(SolutionPath)!.FullName;
        }
        ResultsPath = Path.Combine(SolutionPath, "Results.xlsx");
        UnzippedFolderPath = Path.Combine(SolutionPath, "Unzipped");
        ScriptsPath = Path.Combine(SolutionPath, "Scripts");
        TranslateScriptPath = Path.Combine(ScriptsPath, "Translate.py");
        UnitTestScriptPath = Path.Combine(ScriptsPath, "UnitTester.py");
        ModelFilesPath = Path.Combine(SolutionPath, "ModelFiles");
        ExtractorPath = Path.Combine(ModelFilesPath, "extractor");
        ModDecPath = Path.Combine(ModelFilesPath, "modDec");
        ConverterPath = Path.Combine(ModelFilesPath, "converter");
        MatchFinderScriptPath = Path.Combine(ScriptsPath, "MatchFinder.py");
        PlagiarismResFolder = "C:\\Users\\z004w26z\\Desktop\\res";
        Students = new List<StudentInfo>();
        PlagiarismPairs = new List<PlagiarismPair>();
    }
}