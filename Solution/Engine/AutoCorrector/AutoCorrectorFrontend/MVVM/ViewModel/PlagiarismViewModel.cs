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
        AveragePlahiarismThreshold = 30;
        MaxPlagiarismThreshold = 30;
        FilterPlagiarismPairs(AveragePlahiarismThreshold, true);
        FilterPlagiarismPairs(MaxPlagiarismThreshold, false);
    }

    private void FilterPlagiarismPairs(int value, bool avg)
    {
        PlagiarismPairs.Clear();

        foreach (var pair in Settings.PlagiarismPairs)
        {
            if (avg)
            {
                if (pair.Average_similarity >= value)
                {
                    PlagiarismPairs.Add(pair);
                }
            }
            else
            {
                if (pair.Max_similarity >= value)
                {
                    PlagiarismPairs.Add(pair);
                }
            }
           
        }
    }

    partial void OnAveragePlahiarismThresholdChanging(int value)
    {
        FilterPlagiarismPairs(value, true);
    }

    partial void OnMaxPlagiarismThresholdChanging(int value)
    {
        FilterPlagiarismPairs(value, false);
    }
}