﻿using AutoCorrectorEngine;
using AutoCorrectorFrontend.MVVM.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class SettingsViewModel : ObservableObject
{
    private NotificationService _notificationService;
    public SettingsViewModel(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public enum LLMOptions { Local, Server }
    public List<string> Options { get; } = Enum.GetNames(typeof(LLMOptions)).ToList();
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsServerSelected))]
    private string _selectedOption = LLMOptions.Local.ToString();

    [ObservableProperty]
    private string _selectedServer = "";
    public bool IsServerSelected => SelectedOption == "Server";

    [ObservableProperty]
    private int _plagiarismThreshold = 50;

    partial void OnSelectedServerChanged(string value)
    {
        Settings.LLMRunningLocation = value;
    }

    partial void OnSelectedOptionChanged(string value)
    {
        if (value == "Local")
        {
            Settings.LLMRunningLocation = "Local";
        }
    }

    partial void OnPlagiarismThresholdChanged(int value)
    {
        Settings.PlagiarismThreshold = PlagiarismThreshold;
        _notificationService.NotificationText = $"Plagiarism threshold changed to {Settings.PlagiarismThreshold}";
    }

    [RelayCommand]
    public void ApplyChanges()
    {
        Settings.PlagiarismThreshold = PlagiarismThreshold;
        _notificationService.NotificationText = "Changes Applied!";
    }
}