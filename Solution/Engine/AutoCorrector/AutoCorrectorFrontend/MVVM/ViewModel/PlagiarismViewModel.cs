using System;
using System.Collections.ObjectModel;
using AutoCorrectorEngine;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class PlagiarismViewModel: ObservableObject
{
    public ObservableCollection<PlagiarismPair> PlagiarismPairs { get; set; } = new ObservableCollection<PlagiarismPair>();

    [ObservableProperty]
    private int _plagiarismThreshold;
    public PlagiarismViewModel()
    {
        PlagiarismThreshold = 50;
        FilterPlagiarismPairs(PlagiarismThreshold);
    }

    private void FilterPlagiarismPairs(int value)
    {
        PlagiarismPairs.Clear();

        foreach (var pair in Settings.PlagiarismPairs)
        {
            string plagiarismScore = pair.PlagiarismScore;
            plagiarismScore = plagiarismScore.Replace(" ", "");
            plagiarismScore = plagiarismScore.Replace("%", "");
            int plagScore = int.Parse(plagiarismScore);

            if (plagScore >= value)
            {
                PlagiarismPairs.Add(pair);
            }
        }
    }

    partial void OnPlagiarismThresholdChanging(int value)
    {
        FilterPlagiarismPairs(value);
    }
}