using AutoCorrectorEngine;
using AutoCorrectorFrontend.MVVM.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Windows.Data;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class SettingsViewModel : ObservableObject
{
    private NotificationService _notificationService;
    public SettingsViewModel(NotificationService notificationService)
    {
        _notificationService = notificationService;
        Options = Enum.GetNames(typeof(LLMOptions)).ToList();
        Settings.LLMRunningLocation = Options.Where(x => x.Contains("Server")).ToList()[0];

        string line = "";

        string keyPath = Path.Combine(Settings.SolutionPath, "ApiKey.txt");

        if (File.Exists(keyPath))
        {
            StreamReader sr = new StreamReader(keyPath);

            line = sr.ReadLine();
            sr.Close();

            ApiKey = line;
        }

    }

    public enum LLMOptions { Local, Server }
    public List<string> Options { get; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsServerSelected))]
    private string _selectedOption = "Server";

    [ObservableProperty]
    private string _selectedServer = "";
    public bool IsServerSelected => SelectedOption == "Server";

    [ObservableProperty]
    private int _plagiarismThreshold = 50;

    [ObservableProperty]
    private string? _apiKey;

    partial void OnSelectedOptionChanged(string value)
    {
        Settings.LLMRunningLocation = value;
    }

    [RelayCommand]
    public void ApplyChanges()
    {
        Settings.APIKey = ApiKey;
        using (StreamWriter writer = new StreamWriter(Path.Combine(Settings.SolutionPath, "ApiKey.txt"), false))
        {
            writer.Write(ApiKey);
        }

        _notificationService.NotificationText = "Changes Applied!";
    }
}