using System.Collections.ObjectModel;
using AutoCorrectorEngine;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class PlagiarismViewModel : ObservableObject
{
    public ObservableCollection<PlagiarismChecker.PlagiarismPair> PlagiarismPairs { get; set; } = new ObservableCollection<PlagiarismChecker.PlagiarismPair>();

    [ObservableProperty]
    private int _averagePlahiarismThreshold;

    [ObservableProperty]
    private int _maxPlagiarismThreshold;
    public PlagiarismViewModel()
    {
        AveragePlahiarismThreshold = 50;
        MaxPlagiarismThreshold = 50;
        FilterPlagiarismPairs(AveragePlahiarismThreshold);
    }

    private void FilterPlagiarismPairs(int value)
    {
        PlagiarismPairs.Clear();

        foreach (var pair in Settings.PlagiarismPairs)
        {
            if (pair.First_similarity >= value)
            {
                PlagiarismPairs.Add(pair);
            }
        }
    }

    partial void OnAveragePlahiarismThresholdChanging(int value)
    {
        FilterPlagiarismPairs(value);
    }
}