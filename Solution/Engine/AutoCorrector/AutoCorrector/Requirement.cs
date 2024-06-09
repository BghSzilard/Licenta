using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCorrectorEngine;

public partial class Requirement: ObservableObject
{
    [ObservableProperty] private string _title;
    public ObservableCollection<SubRequirement> SubRequirements { get; set; }
    
    [ObservableProperty] private float _points;

    [ObservableProperty] private string _type;

    [ObservableProperty] private int _line;
    public Requirement()
    {
        Title = string.Empty;
        SubRequirements = new ObservableCollection<SubRequirement>();
    }
}