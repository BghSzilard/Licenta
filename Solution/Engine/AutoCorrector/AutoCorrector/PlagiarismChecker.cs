using AutoCorrector;

namespace AutoCorrectorEngine;

public class PlagiarismChecker
{
    public async Task<List<PlagiarismPair>> CheckPlagiarism(List<StudentInfo> students)
    {
        ProcessExecutor processExecutor = new ProcessExecutor();

        await processExecutor.ExecuteProcess("powershell", $"java -jar C:\\Users\\z004w26z\\Desktop\\jplag.jar --csv-export -l cpp --result-file={Settings.PlagiarismResFolder}", $"{Settings.UnzippedFolderPath}");

        string fileName = "results.csv";
        string filePath = Path.Combine(Settings.PlagiarismResFolder, fileName);

        List<PlagiarismPair> plagiarismPairs = new List<PlagiarismPair>();

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string headerLine = reader.ReadLine();
                // Read the header line if needed
                // string headerLine = reader.ReadLine();

                // Read the rest of the lines
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    // Split the line by comma to get values
                    string[] values = line.Split(',');

                    var firstStud = students.First(x => values[0].Contains(x.Name));
                    var secondStud = students.First(x => values[1].Contains(x.Name));
                    float value = float.Parse(values[2]);
                    value = value * 100;
                    int newVal = (int)Math.Round(value);
                    string score = newVal.ToString() + "%";
                    plagiarismPairs.Add(new PlagiarismPair { FirstStudent = firstStud.Name, SecondStudent = secondStud.Name, PlagiarismScore = score });
                    
                }
            }
        }

        return plagiarismPairs;
    }
}