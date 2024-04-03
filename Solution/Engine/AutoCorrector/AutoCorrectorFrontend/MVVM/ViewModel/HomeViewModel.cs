using System.IO;
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
    public bool DependenciesUploaded => _uploadedScale != "None" && _uploadedZip != "None";
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

    private async Task RunSetting(string name, string path)
    {
        string fileContent = File.ReadAllText(path);
        string apiLocation = "/api/create";
        apiLocation = apiLocation.Insert(0, Settings.LLMRunningLocation);

        string script = $@"$modelfileContent = Get-Content -Path ""{path}"" -Raw

            $body = @{{
              ""name"" = ""{name}""
              ""modelfile"" = $modelfileContent | Out-String
            }} | ConvertTo-Json
            
            Invoke-WebRequest -Uri {apiLocation} -Method Post -Body $body -ContentType ""application/json""
            ";

        //string script = $@"
        //$response = Invoke-RestMethod -Method Post -Uri '{apiLocation}' -ContentType 'application/json' -Body (@{{
        //    name = '{name}'
        //    modelfile = '{fileContent}'
        //}} | ConvertTo-Json)

        //$response";
        ProcessExecutor processExecutor = new ProcessExecutor();
        await processExecutor.ExecuteProcess("powershell.exe", "-Command \"& {" + script + "}\"", "");
    }

    [RelayCommand]
    public async Task GradeProjects()
    {
        //if (Settings.LLMRunningLocation != "Local")
        //{
        //    _notificationService.NotificationText = "Setting Up Models...";
        //    await RunSetting("extractor", Settings.ExtractorPath);
        //    await RunSetting("converter", Settings.ConverterPath);
        //    await RunSetting("moddec", Settings.ModDecPath);
        //}
        
        StudentManager studentManager = new StudentManager(_notificationService, UploadedZip, UploadedScale);
        await studentManager.Solve();
        _notificationService.NotificationText = "Results Saved!";
    }
}