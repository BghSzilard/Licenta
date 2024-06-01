using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCorrectorFrontend.MVVM.Services;
using System.Timers;

public partial class NotificationService : ObservableObject
{
    private string? _notificationText;

    public string? NotificationText
    {
        get => _notificationText;
        set
        {
            if (_notificationText != value)
            {
                _notificationText = value;
                OnPropertyChanged(nameof(NotificationText));
            }
        }
    }
}