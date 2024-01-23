namespace AutoCorrector;
public class StudentManager
{
    private readonly ExcelManager _excelManager;
    private readonly FileProcessor _fileProcessor;
    private readonly List<StudentInfo> _students;
    private readonly string _resultsPath;
    private readonly string _zipPath;
    private readonly string _unzippedFolderPath;
    public StudentManager() 
    {
        _excelManager = new ExcelManager();
        _fileProcessor = new FileProcessor();
        _students = new List<StudentInfo>();
        _resultsPath = "C:\\Users\\sziba\\Desktop\\New folder\\rezultate.xlsx";
        _zipPath = "C:\\Users\\sziba\\Desktop\\AF_2023-Testare Parțial 1 - 17 noiembrie 1600-53027.zip";
        _unzippedFolderPath = "C:\\Users\\sziba\\Desktop\\MyFolder";
    }

    public async Task GetNames()
    {
        UnzipFile();
        GetStudentNames();
        await SaveResults();
    }
    private void GetStudentNames()
    {
        FileProcessor fileProcessor = new FileProcessor();
        var subdirectoryNames = fileProcessor.GetSubdirectoryNames(_unzippedFolderPath);

        foreach (var subdirectoryName in subdirectoryNames)
        {
            var studentName = fileProcessor.SeparateString(subdirectoryName, '_');
            _students.Add(new StudentInfo() { Name = studentName });
        }
    }
    private void UnzipFile()
    {
        _fileProcessor.ExtractZip(_zipPath, _unzippedFolderPath);
    }
    private async Task SaveResults()
    {
        FileInfo fileInfo = new FileInfo(_resultsPath);
        await _excelManager.SaveExcelFile(_students, fileInfo);
    }
}