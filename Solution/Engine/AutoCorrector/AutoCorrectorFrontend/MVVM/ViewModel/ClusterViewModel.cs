using System.Collections.ObjectModel;
using AutoCorrector;
using AutoCorrectorEngine;
using CommunityToolkit.Mvvm.ComponentModel;
using static AutoCorrectorEngine.PlagiarismChecker;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public class ClusterViewModel : ObservableObject
{
    public PlagiarismPair Pair { get; set; }
    public ObservableCollection<StudentInfo> Students { get; set; } = new ObservableCollection<StudentInfo>();
    public ObservableCollection<PlagiarismPair> PlagiarismPairs { get; set; } = new ObservableCollection<PlagiarismPair>();
    public ClusterViewModel(PlagiarismPair plagiarismPair)
    {
        Pair = plagiarismPair;
        List<int> ids = new List<int>();

        foreach (var id in Pair.Cluster)
        {
            StudentInfo student = Settings.Students.First(x => x.Id == id);
            Students.Add(student);
        }

        List<string> studentNames = Students.Select(x => x.Name).ToList();

        foreach (var pair in Settings.PlagiarismPairs)
        {
            if (studentNames.Contains(pair.Id1) && studentNames.Contains(pair.Id2))
            {
                PlagiarismPairs.Add(pair);
            }
        }
    }
}