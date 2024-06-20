using AutoCorrector;

namespace AutoCorrectorEngine;

public class Settings
{
    public static string APIKey { get; set; } = "";
    public static string LLMRunningLocation { get; set; } = "Local";
    public static string ZipPath { get; set; } = "";
    public static string ProjectsPath { get; set; } = "";
    public static string SolutionPath { get; set; }
    public static string ModelFilesPath { get; set; }
    public static string ExtractorPath { get; set; }
    public static string ModDecPath { get; set; }
    public static string ConverterPath { get; set; }
    public static string ResultsPath  { get; set; }
    public static string UnzippedFolderPath { get; set; }
    public static int PlagiarismThreshold {  get; set; }
    public static StudentInfo StudentSample {  get; set; }
    public static List<StudentInfo> Students { get; set; }
    public static string PlagiarismResZip {  get; set; }
    public static List<Requirement> Requirements { get; set; } = new List<Requirement>();
    public static string PlagiarismResFolder {  get; set; }
    public static string JplagPath {  get; set; }
    public static List<PlagiarismChecker.PlagiarismPair> PlagiarismPairs { get; set; }
    public static string SyntaxPath;
    public static string UnitTestsPath;
    public static List<List<double>> AdjacencyMatrixStudSim = new List<List<double>>();
    public static List<List<int>> Clusters;
    static Settings()
    {
        SolutionPath = Directory.GetCurrentDirectory();
        for (int i = 0; i < 4; ++i)
        {
            SolutionPath = Directory.GetParent(SolutionPath)!.FullName;
        }
        JplagPath = Path.Combine(SolutionPath, "Jplag//jplag.jar");
        ResultsPath = Path.Combine(SolutionPath, "Results.xlsx");
        UnzippedFolderPath = Path.Combine(SolutionPath, "Unzipped");
        ModelFilesPath = Path.Combine(SolutionPath, "ModelFiles");
        ExtractorPath = Path.Combine(ModelFilesPath, "extractor");
        ModDecPath = Path.Combine(ModelFilesPath, "modDec");
        ConverterPath = Path.Combine(ModelFilesPath, "converter");
        PlagiarismResZip = Path.Combine(SolutionPath, "res.zip");
        PlagiarismResFolder = Path.Combine(SolutionPath, "res");
        Students = new List<StudentInfo>();
        PlagiarismPairs = new List<PlagiarismChecker.PlagiarismPair>();
        SyntaxPath = Path.Combine(SolutionPath, "Syntax");
        SyntaxPath = Path.Combine(SyntaxPath, "cpp.xshd");
        UnitTestsPath = Path.Combine(SolutionPath, "UnitTests");
        LLMRunningLocation = "Server";

        string line;
        string keyPath = Path.Combine(Settings.SolutionPath, "ApiKey.txt");

        if (File.Exists(keyPath))
        {
            StreamReader sr = new StreamReader(keyPath);

            line = sr.ReadLine();

            sr.Close();

            APIKey = line;
        }
    }
}