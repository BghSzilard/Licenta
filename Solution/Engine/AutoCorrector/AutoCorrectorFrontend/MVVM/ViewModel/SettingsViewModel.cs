using AutoCorrectorEngine;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class SettingsViewModel : ObservableObject
{
    public enum LLMOptions { Local, Server }
    public List<string> Options { get; } = Enum.GetNames(typeof(LLMOptions)).ToList();
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsServerSelected))]
    private string _selectedOption = LLMOptions.Local.ToString();

    [ObservableProperty]
    private string _selectedServer = "";
    public bool IsServerSelected => SelectedOption == "Server";

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
}