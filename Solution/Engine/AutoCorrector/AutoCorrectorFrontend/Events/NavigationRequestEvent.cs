namespace AutoCorrectorFrontend.Events;

public class NavigationRequestEvent
{
    public Type ViewModelType { get; }

    public NavigationRequestEvent(Type viewModelType)
    {
        ViewModelType = viewModelType;
    }
}