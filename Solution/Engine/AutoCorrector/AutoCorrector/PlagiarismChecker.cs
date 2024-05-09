using System.Collections.ObjectModel;
using System.Diagnostics;
using AutoCorrector;
using CommunityToolkit.Mvvm.ComponentModel;
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
        [ObservableProperty] private string file1;
        [ObservableProperty] private string file2;
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

        [ObservableProperty] private string _sourceFile1;
        [ObservableProperty] private string _sourceFile2;

        public ObservableCollection<int> Cluster { get; set; } = new ObservableCollection<int>();
    }
    public async Task<List<PlagiarismPair>> CheckPlagiarism(List<StudentInfo> students)
    {
        ProcessExecutor processExecutor = new ProcessExecutor();

        //if (File.Exists(Settings.PlagiarismResFolder))
        //{
        //    File.Delete(Settings.PlagiarismResFolder);
        //}

        //Process process = new Process();
        //// Configure the process using the StartInfo properties.
        //process.StartInfo.FileName = "cmd";
        //process.StartInfo.Arguments = "java -jar C:\\Users\\z004w26z\\Desktop\\jplag.jar -l cpp --result-file=C:\\Users\\z004w26z\\Desktop\\res.zip C:\\Users\\z004w26z\\Desktop\\Material\\Licenta\\Licenta\\Solution\\Engine\\AutoCorrector\\Unzipped";
        //process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
        //process.StartInfo.UseShellExecute = false;
        //process.StartInfo.CreateNoWindow = true;
        //process.Start();

        //await process.WaitForExitAsync();

        //await processExecutor.ExecuteProcess("powershell", $"\"C:\\Program Files\\Common Files\\Oracle\\Java\\javapath\\java.exe\" -jar C:\\Users\\z004w26z\\Desktop\\jplag.jar -l cpp --result-file={Settings.PlagiarismResFolder} {Settings.UnzippedFolderPath}", "");


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

                    string sourceFile1 = students.First(x => deserializedJson.id1.Contains(x.Name)).SourceFile!;
                    string sourceFile2 = students.First(x => deserializedJson.id2.Contains(x.Name)).SourceFile!;

                    using (StreamReader sr = new StreamReader(sourceFile1))
                    {
                        // Read the entire file
                        string fileContent = sr.ReadToEnd();

                        pair.SourceFile1 = fileContent;
                    }

                    using (StreamReader sr = new StreamReader(sourceFile2))
                    {
                        // Read the entire file
                        string fileContent = sr.ReadToEnd();

                        pair.SourceFile2 = fileContent;
                    }

                    foreach (var match in deserializedJson.matches)
                    {
                        ObservableMatch observableMatch = new ObservableMatch();
                        observableMatch.Start1 = match.start1;
                        observableMatch.Start2 = match.start2;
                        observableMatch.End1 = match.end1;
                        observableMatch.End2 = match.end2;
                        observableMatch.File1 = match.file1;
                        observableMatch.File2 = match.file2;
                        pair.matches.Add(observableMatch);
                    }
                    plagiarismPairs.Add(pair);
                }
            }
        }
        return plagiarismPairs;
    }
}