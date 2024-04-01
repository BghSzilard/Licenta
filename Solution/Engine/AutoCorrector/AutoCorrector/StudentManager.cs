using AutoCorrectorEngine;
using AutoCorrectorFrontend.MVVM.Services;

namespace AutoCorrector;
public class StudentManager
{
    private readonly ExcelManager _excelManager;
    private readonly FileProcessor _fileProcessor;
    private readonly List<StudentInfo> _students;
    private readonly NotificationService _notificationService;
    public StudentManager(NotificationService notificationService) 
    {
        _excelManager = new ExcelManager();
        _fileProcessor = new FileProcessor();
        _students = new List<StudentInfo>();
        _notificationService = notificationService;
    }
    public async Task Solve()
    {
        _notificationService.NotificationText = "Unzipping Projects...";
        await UnzipFile();
        _notificationService.NotificationText = "Projects Unzipped!";
        var folders = Directory.GetDirectories(Settings.UnzippedFolderPath);

        _notificationService.NotificationText = "Extracting Source Files...";

        foreach (var folder in folders)
        {
            await ExtractEssence(folder);
        }

        _notificationService.NotificationText = "Source Files Extracted!";

        GetStudentNames();
        await CheckCompilations();
        _notificationService.NotificationText = "Saving Results...";
        await SaveResults();
    }

    private async Task CheckCompilations()
    {
        foreach (var student in _students)
        {
            var folderPath = _fileProcessor.GetFolder(Settings.UnzippedFolderPath, student.Name);
            var sourceFile = await _fileProcessor.FindSourceFile(folderPath);

            if (sourceFile != null)
            {
                student.CodeCompiles = _fileProcessor.Compiles(sourceFile);
            }
            else
            {
                student.CodeCompiles = false;
            }

            _notificationService.NotificationText = $"Compilation check made for {student.Name}";
        }
    }
    private async Task ExtractEssence(string path)
    {
        await _fileProcessor.ExtractArchivesRecursively(path);

        var subdirectories = Directory.GetDirectories(path);
        List<string> extensions = new() { ".txt", ".h", ".hpp", ".cpp" };

        DirectoryInfo directoryInfo = new(path);

        var files = directoryInfo.GetFiles();

        foreach (var file in files)
        {
            string extension = file.Extension;

            if (!extensions.Contains(extension))
            {
                file.Delete();
            }
        }

        foreach (var subdirectory in subdirectories)
        {
            _fileProcessor.ExtractFiles(path, subdirectory, extensions);
        }
    }

    private void GetStudentNames()
    {
        FileProcessor fileProcessor = new FileProcessor();
        var subdirectoryNames = fileProcessor.GetSubdirectoryNames(Settings.UnzippedFolderPath);

        foreach (var subdirectoryName in subdirectoryNames)
        {
            var studentName = fileProcessor.SeparateString(subdirectoryName, '_');
            _students.Add(new StudentInfo() { Name = studentName });
        }
    }
    private async Task UnzipFile()
    {
        await _fileProcessor.ExtractZip(Settings.ZipPath, Settings.UnzippedFolderPath);
    }
    private async Task SaveResults()
    {
        FileInfo fileInfo = new FileInfo(Settings.ResultsPath);
        await _excelManager.SaveExcelFile(_students, fileInfo);
    }
}