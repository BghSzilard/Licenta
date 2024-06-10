using System.Collections.ObjectModel;
using System.Diagnostics;
using AutoCorrector;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using SharpCompress.Common;

namespace AutoCorrectorEngine;

public partial class PlagiarismChecker
{
    public class Match
    {
        public string file1 { get; set; }
        public string file2 { get; set; }
        public int start1 { get; set; }
        public int end1 { get; set; }
        public int start2 { get; set; }
        public int end2 { get; set; }
        public int tokens { get; set; }
    }

    public class Root
    {
        public string id1 { get; set; }
        public string id2 { get; set; }
        public Dictionary<string, double> similarities { get; set; }
        public List<Match> matches { get; set; }
        public double first_similarity { get; set; }
        public double second_similarity { get; set; }
    }

    public partial class ObservableMatch : ObservableObject
    {
        [ObservableProperty] private int _id;
        [ObservableProperty] private SourceFile file1;
        [ObservableProperty] private SourceFile file2;
        [ObservableProperty] private int start1;
        [ObservableProperty] private int end1;
        [ObservableProperty] private int start2;
        [ObservableProperty] private int end2;
        [ObservableProperty] private int tokens;
    }
    public partial class PlagiarismPair : ObservableObject
    {

        [ObservableProperty] private string _id1;
        [ObservableProperty] private string _id2;
        public ObservableCollection<ObservableMatch> matches { get; set; }
        [ObservableProperty] private int first_similarity;
        [ObservableProperty] private int second_similarity;

        [ObservableProperty] private int average_similarity;
        [ObservableProperty] private int max_similarity;

        public ObservableCollection<SourceFile> Files1 = new ObservableCollection<SourceFile>();
        public ObservableCollection<SourceFile> Files2 = new ObservableCollection<SourceFile>();
        public ObservableCollection<int> Cluster { get; set; } = new ObservableCollection<int>();
    }

    public async Task<List<PlagiarismPair>> CheckPlagiarism(List<StudentInfo> students)
    {
        ProcessExecutor processExecutor = new ProcessExecutor();

        if (Directory.Exists("C:\\Users\\z004w26z\\Desktop\\res"))
        {
            Directory.Delete("C:\\Users\\z004w26z\\Desktop\\res", true);
        }

        if (File.Exists("C:\\Users\\z004w26z\\Desktop\\res.zip"))
        {
            File.Delete("C:\\Users\\z004w26z\\Desktop\\res.zip");
        }

        Process process = new Process();

        process.StartInfo.WorkingDirectory = @"C:\Users\z004w26z\Desktop\Material\Licenta\Licenta\Solution\Engine\AutoCorrector";

        
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        process.StartInfo.FileName = "\"C:\\Program Files\\Common Files\\Oracle\\Java\\javapath\\java.exe\"";
        process.StartInfo.Arguments = "-jar C:\\Users\\z004w26z\\Desktop\\jplag.jar -l cpp --result-file=C:\\Users\\z004w26z\\Desktop\\res.zip C:\\Users\\z004w26z\\Desktop\\Material\\Licenta\\Licenta\\Solution\\Engine\\AutoCorrector\\Unzipped";
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.Start();

        await Task.Run(() =>
        {
            process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd(); // Read standard output
            string error = process.StandardError.ReadToEnd(); // Read standard error

        });

        List<PlagiarismPair> plagiarismPairs = new List<PlagiarismPair>();

        if (File.Exists(Settings.PlagiarismResFolder))
        {
            FileProcessor fileProcessor = new FileProcessor();

            string extractedPath = Settings.PlagiarismResFolder;
            extractedPath = extractedPath.Replace(".zip", "");

            await fileProcessor.ExtractZip(Settings.PlagiarismResFolder, extractedPath);

            var excludedFiles = new[] { "options.json", "overview.json", "submissionFileIndex.json" };

            var jsonFiles = Directory.EnumerateFiles(extractedPath, "*.json")
                                     .Where(file => !excludedFiles.Contains(Path.GetFileName(file)));

            foreach (var jsonFile in jsonFiles)
            {
                HashSet<string> alreadyUsedFiles = new HashSet<string>();

                using (StreamReader file = File.OpenText(jsonFile))
                {
                    Root deserializedJson = JsonConvert.DeserializeObject<Root>(file.ReadToEnd());

                    deserializedJson.id1 = students.First(x => deserializedJson.id1.Contains(x.Name)).Name;
                    deserializedJson.id2 = students.First(x => deserializedJson.id2.Contains(x.Name)).Name;

                    PlagiarismPair pair = new PlagiarismPair();
                    pair.matches = new ObservableCollection<ObservableMatch>();
                    pair.Id1 = students.First(x => deserializedJson.id1.Contains(x.Name)).Name;
                    pair.Id2 = students.First(x => deserializedJson.id2.Contains(x.Name)).Name;
                    pair.First_similarity = (int)Math.Round(deserializedJson.first_similarity * 100);
                    pair.Second_similarity = (int)Math.Round(deserializedJson.second_similarity * 100);
                    pair.Max_similarity = Math.Max(pair.First_similarity, pair.Second_similarity);
                    pair.Average_similarity = (pair.First_similarity + pair.Second_similarity) / 2;

                    int i = 1;
                    int j = 1;
                    int matchId = 1;
                    alreadyUsedFiles.Clear();
                    foreach (var match in deserializedJson.matches)
                    {
                        
                        string filePath1 = Path.Combine(extractedPath, "files", match.file1);
                        string filePath2 = Path.Combine(extractedPath, "files", match.file2);
                        
                        if (!alreadyUsedFiles.Contains(match.file1))
                        {
                            using (StreamReader sr = new StreamReader(filePath1))
                            {
                                string fileContent = sr.ReadToEnd();

                                SourceFile sourceFile = new SourceFile();
                                sourceFile.Name = match.file1;
                                sourceFile.Content = fileContent;
                                sourceFile.Id = i;

                                pair.Files1.Add(sourceFile);
                                ++i;
                            }

                            alreadyUsedFiles.Add(match.file1);
                        }
                       

                        if (!alreadyUsedFiles.Contains(match.file2))
                        {
                            using (StreamReader sr = new StreamReader(filePath2))
                            {
                                string fileContent = sr.ReadToEnd();

                                SourceFile sourceFile = new SourceFile();
                                sourceFile.Name = match.file2;
                                sourceFile.Content = fileContent;
                                sourceFile.Id = j;

                                pair.Files2.Add(sourceFile);
                                ++j;
                            }

                            alreadyUsedFiles.Add(match.file2);
                        }

                        ObservableMatch observableMatch = new ObservableMatch();
                        observableMatch.Start1 = match.start1;
                        observableMatch.Start2 = match.start2;
                        observableMatch.End1 = match.end1;
                        observableMatch.End2 = match.end2;
                        observableMatch.File1 = pair.Files1[pair.Files1.Count - 1];
                        observableMatch.File2 = pair.Files2[pair.Files2.Count - 1];
                        observableMatch.Id = matchId;
                        pair.matches.Add(observableMatch);
                        matchId++;
                    }

                    plagiarismPairs.Add(pair);
                }
            }
        }
        return plagiarismPairs;
    }
}