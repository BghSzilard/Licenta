using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class MainViewModel : ObservableObject
{
    [RelayCommand]
    public void NavigateHome()
    {
        CurrentView = new HomeViewModel();
    }
    [RelayCommand]
    public void NavigateDiscovery()
    {
        CurrentView = new Discovery();
    }

    [RelayCommand]
    public void NavigateSettings()
    {
        CurrentView = new SettingsViewModel();
    }

    [ObservableProperty]
    private ObservableObject _currentView;
    public MainViewModel()
    {
        CurrentView = new HomeViewModel();
    }
}