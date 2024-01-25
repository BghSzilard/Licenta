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
        _unzippedFolderPath = "C:\\Users\\sziba\\Desktop\\MyFolder2";
    }
    public async Task Solve()
    {
        //UnzipFile();

        //var folders = Directory.GetDirectories(_unzippedFolderPath);

        //foreach (var folder in folders)
        //{
        //    ExtractEssence(folder);
        //}

        GetStudentNames();
        CheckCompilations();
        await SaveResults();
    }

    private void CheckCompilations()
    {
        foreach (var student in _students)
        {
            var folderPath = _fileProcessor.GetFolder(_unzippedFolderPath, student.Name);
            var sourceFile = _fileProcessor.FindSourceFile(folderPath);

            if (sourceFile != null)
            {
                student.CodeCompiles = _fileProcessor.Compiles(sourceFile);
            }
            else
            {
                student.CodeCompiles = false;
            }
        }
    }
    private void ExtractEssence(string path)
    {
        _fileProcessor.ExtractArchivesRecursively(path);

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