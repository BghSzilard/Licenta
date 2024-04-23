using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCorrectorEngine
{
    public class PlagiarismPair: ObservableObject
    {
        public string FirstStudent {get; set;}
        public string SecondStudent {get; set;}
        public string PlagiarismScore { get; set;}
    }
}