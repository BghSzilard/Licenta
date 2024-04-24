using static AutoCorrectorEngine.PlagiarismChecker;

namespace AutoCorrectorFrontend.Events;

public class NavigationRequestEvent
{
    public Type ViewModelType { get; }

    public PlagiarismPair PlagiarismPair { get; set; }
    public NavigationRequestEvent(Type viewModelType, PlagiarismPair plagiarismPair)
    {
        ViewModelType = viewModelType;
        PlagiarismPair = plagiarismPair;
    }
}