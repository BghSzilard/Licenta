using System.Collections.ObjectModel;
using AutoCorrectorEngine;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCorrector;

public partial class StudentInfo: ObservableObject
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private float _grade = 1;

    [ObservableProperty] 
    private bool _codeCompiles;
    public string? SourceFile { get; set; }

    public List<string> HeaderFiles { get; set; } = new List<string>();

    public int Id { get; set; }
    public ObservableCollection<Requirement> Requirements { get; set; } = new ObservableCollection<Requirement>();
}