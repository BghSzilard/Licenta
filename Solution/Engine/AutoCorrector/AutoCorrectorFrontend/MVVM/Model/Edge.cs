using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCorrectorFrontend.MVVM.Model;

public class Edge: ObservableObject
{
    public string Name1 { get; set; } = "";
    public string Name2 { get; set; } = "";
}