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
    [ObservableProperty]
    private ObservableObject _currentView;
    public MainViewModel()
    {
        NotificationService = new NotificationService();
        CurrentView = new HomeViewModel(NotificationService);
    }
}