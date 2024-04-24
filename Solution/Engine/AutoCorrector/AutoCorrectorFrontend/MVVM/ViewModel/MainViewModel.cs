using AutoCorrectorFrontend.Events;
using AutoCorrectorFrontend.MVVM.Services;
using Caliburn.Micro;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class MainViewModel : ObservableObject, IHandle<NavigationRequestEvent>
{
    private readonly IEventAggregator _eventAggregator;
    public NotificationService NotificationService { get; }
    [RelayCommand]
    public void NavigateHome()
    {
        CurrentView = new HomeViewModel(NotificationService);
    }
    [RelayCommand]
    public void NavigateDiscovery()
    {
        CurrentView = new ResultsViewModel();
    }

    [RelayCommand]
    public void NavigateSettings()
    {
        CurrentView = new SettingsViewModel(NotificationService);
    }

    [RelayCommand]
    public void NavigatePlagiarism()
    {
        CurrentView = new PlagiarismViewModel();
    }

    public Task HandleAsync(NavigationRequestEvent message, CancellationToken cancellationToken)
    {
        if (message.ViewModelType == typeof(PlagiarismViewModel))
        {
            CurrentView = new ComparisonViewModel(message.PlagiarismPair);
        }

        return Task.CompletedTask;
    }

    [ObservableProperty]
    private ObservableObject _currentView;
    public MainViewModel(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
        _eventAggregator.SubscribeOnPublishedThread(this);
        NotificationService = new NotificationService();
        CurrentView = new HomeViewModel(NotificationService);
    }
}