using AutoCorrectorEngine;
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
    public bool DependenciesUploaded => _uploadedScale != "None" && _uploadedZip != "None";
    public HomeViewModel()
    {
        UploadedScale = "None";
        UploadedZip = "None";
    }

    [RelayCommand]
    public void OpenScale()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "XML files (*.xml)|*.xml";
        if (openFileDialog.ShowDialog() == true)
        {
            string selectedFileName = openFileDialog.FileName;
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
            UploadedZip = selectedFileName;
        }
    }

    [RelayCommand]
    public void GradeProjects()
    {
        ScaleProcessor scaleProcessor = new ScaleProcessor();
        scaleProcessor.ProcessScale(UploadedScale);
    }
}