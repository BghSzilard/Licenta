using AutoCorrectorEngine;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class ScaleCreatorViewModel: ObservableObject
{
    [ObservableProperty]
    private string _uploadedDocument;

    [RelayCommand]
    public void OpenDocument()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Doc files (*.pdf, *.docx, *.txt)|*.pdf;*.txt;*.docx";
        if (openFileDialog.ShowDialog() == true)
        {
            string selectedFileName = openFileDialog.FileName;
            Settings.ProjectsPath = selectedFileName;
            UploadedDocument = selectedFileName;
        }
    }
}