﻿using AutoCorrectorFrontend.MVVM.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class MainViewModel : ObservableObject
{
    public NotificationService NotificationService { get; }
    [RelayCommand]
    public void NavigateHome()
    {
        CurrentView = new HomeViewModel();
        NotificationService.NotificationText = "Navigated to Home";
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
        NotificationService.NotificationText = "Navigated to Settings";
    }

    [ObservableProperty]
    private ObservableObject _currentView;
    public MainViewModel()
    {
        CurrentView = new HomeViewModel();
        NotificationService = new NotificationService();
    }
}