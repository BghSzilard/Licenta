using AutoCorrector;
using AutoCorrectorEngine;
using AutoCorrectorFrontend.MVVM.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace AutoCorrectorFrontend.MVVM.ViewModel;
public partial class HomeViewModel : ObservableObject
{

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DependenciesUploaded))]
    private string _uploadedScale;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DependenciesUploaded))]

    private string _uploadedZip;
    public bool DependenciesUploaded => UploadedScale != "None" && UploadedZip != "None";
    private NotificationService _notificationService { get; set; }
    public HomeViewModel(NotificationService notificationService)
    {
        UploadedScale = "None";
        UploadedZip = "None";
        _notificationService = notificationService;
    }

    [RelayCommand]
    public void OpenScale()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "XML files (*.xml)|*.xml";
        if (openFileDialog.ShowDialog() == true)
        {
            string selectedFileName = openFileDialog.FileName;
            Settings.ProjectsPath = selectedFileName;
            UploadedScale = selectedFileName;
        }
    }


    [RelayCommand]
    public void OpenProjects()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Zip files (*.zip)|*.zip";
        if (openFileDialog.ShowDialog() == true)
        {
            string selectedFileName = openFileDialog.FileName;
            Settings.ZipPath = selectedFileName;
            UploadedZip = selectedFileName;
        }
    }

    [RelayCommand]
    public async Task GradeProjects()
    {
        StudentManager studentManager = new StudentManager(_notificationService, UploadedZip, UploadedScale);
        await studentManager.Solve();
        _notificationService.NotificationText = "Students Graded!";
    }
}