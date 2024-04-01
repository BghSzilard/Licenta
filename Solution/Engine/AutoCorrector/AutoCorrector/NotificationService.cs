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

                //// Start a timer to clear the notification after 5 seconds
                //var timer = new Timer(5000);
                //timer.Elapsed += (sender, e) =>
                //{
                //    NotificationText = null;
                //    timer.Dispose(); // Dispose the timer to prevent it from running again
                //};
                //timer.AutoReset = false; // Ensure it only triggers once
                //timer.Start();
            }
        }
    }
}