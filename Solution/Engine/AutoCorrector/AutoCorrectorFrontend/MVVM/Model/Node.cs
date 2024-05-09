using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCorrectorFrontend.MVVM.Model
{
    public class Node: ObservableObject
    {
        public double X { get; set; }
        public double Y { get; set; }

        public string Name { get; set; } = "";
    }
}
