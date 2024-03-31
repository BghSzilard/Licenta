using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class HomeViewModel: ObservableObject
{
    [ObservableProperty]
    private string _uploadedScale;

    [ObservableProperty]
    private string _uploadedZip;

    public HomeViewModel() 
    {
        UploadedScale = "None";
        UploadedZip = "None";
    }

    [RelayCommand]
    public void OpenScale()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
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
        openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            string selectedFileName = openFileDialog.FileName;
            UploadedZip = selectedFileName;
        }
    }
}