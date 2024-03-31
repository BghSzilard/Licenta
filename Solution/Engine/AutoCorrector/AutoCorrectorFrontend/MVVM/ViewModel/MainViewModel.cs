﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class MainViewModel: ObservableObject
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

    [ObservableProperty]
    private ObservableObject _currentView;
    public MainViewModel()
    {
        CurrentView = new HomeViewModel();
    }
}