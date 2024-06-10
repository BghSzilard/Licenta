using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCorrectorFrontend.MVVM.Model;

public partial class Square: ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;
    public int Number {  get; set; }
}