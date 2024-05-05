using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCorrectorEngine;

public partial class SubRequirement: ObservableObject
{
    [ObservableProperty]
    private string? _type;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private float _points;
    public SubRequirement()
    {
        Title = string.Empty;
        Type = string.Empty;
    }
}