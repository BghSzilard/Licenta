using AutoCorrectorFrontend.MVVM.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class MainViewModel : ObservableObject
{
    public NotificationService NotificationService { get; }
    [RelayCommand]
    public void NavigateHome()
    {
        CurrentView = new HomeViewModel(NotificationService);
    }
    [RelayCommand]
    public void NavigateDiscovery()
    {
        CurrentView = new Discovery();
    }

    [RelayCommand]
    public void NavigateSettings()
    {
        CurrentView = new SettingsViewModel(NotificationService);
    }

    [ObservableProperty]
    private ObservableObject _currentView;
    public MainViewModel()
    {
        NotificationService = new NotificationService();
        CurrentView = new HomeViewModel(NotificationService);
    }
}