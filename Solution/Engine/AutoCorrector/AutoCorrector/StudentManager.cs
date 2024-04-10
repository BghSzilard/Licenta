using AutoCorrectorEngine;
using AutoCorrectorFrontend.MVVM.Services;

namespace AutoCorrector;
public class StudentManager
{
    private readonly ExcelManager _excelManager;
    private readonly FileProcessor _fileProcessor;
    private readonly List<StudentInfo> _students;
    private readonly NotificationService _notificationService;
    private string _scale { get; set; }
    private string _zip { get; set; }
    public StudentManager(NotificationService notificationService, string uploadedZip, string scale) 
    {
        _excelManager = new ExcelManager();
        _fileProcessor = new FileProcessor();
        _students = new List<StudentInfo>();
        _notificationService = notificationService;
        _scale = scale;
        _zip = uploadedZip;
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

        ScaleProcessor scaleprocessor = new ScaleProcessor(_notificationService);
        var processedscale = await scaleprocessor.ProcessScale(_scale);

        await GradeStudents(processedscale);
        _notificationService.NotificationText = "Saving Results...";
        await SaveResults();
    }

    private async Task CheckCompilations()
    {
        ProcessExecutor processExecutor = new ProcessExecutor();
        foreach (var student in _students)
        {
            var folderPath = _fileProcessor.GetFolder(Settings.UnzippedFolderPath, student.Name);
            var sourceFile = await _fileProcessor.FindSourceFile(folderPath);
           
            if (sourceFile != null)
            {
                //student.CodeCompiles = _fileProcessor.Compiles(sourceFile);
                if (await processExecutor.ExecuteProcess("powershell.exe", "clang++", sourceFile) == "")
                {
                    student.CodeCompiles = true;
                }

                student.SourceFile = sourceFile;
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

    public async Task GradeStudents(List<Requirement> processedScalde)
    {
        _notificationService.NotificationText = "Grading Students...";
        foreach (var student in _students)
        {
            if (!student.CodeCompiles)
            {
                continue;
            }

            foreach (var requirement in processedScalde)
            {
                FunctionSignatureExtractorWrapper functionSignatureExtractor = new FunctionSignatureExtractorWrapper();
                var signatures = functionSignatureExtractor.GetSignatures(student.SourceFile);

                string allSignatures = "";
                foreach (var signature in signatures)
                {
                    allSignatures += signature;
                    allSignatures += ";";
                }
                allSignatures = allSignatures.Remove(allSignatures.Length - 1);

                ProcessExecutor executor = new ProcessExecutor();
                requirement.Title = requirement.Title.Replace("\"", ""); ;
                var args = $"python '{Settings.MatchFinderScriptPath}' '{allSignatures}' '{requirement.Title}'";
                var functionMatch = await executor.ExecuteProcess("powershell", args, "");
                functionMatch = functionMatch.Replace("\n", "");
                functionMatch = functionMatch.Replace("\r", "");
                FunctionExtractorWrapper functionExtractor = new FunctionExtractorWrapper();

                FileProcessor fileProcessor = new FileProcessor();
                var includes = fileProcessor.FindIncludes(student.SourceFile!);
                var function = includes;
                var extractedFunction = functionExtractor.GetFunction(student.SourceFile, functionMatch);
                function += extractedFunction;

                Requirement studReq = new Requirement();
                studReq.Title = functionMatch;

                student.Requirements.Add(studReq);

                CorrectionChecker checker = new CorrectionChecker();
                var tempFile = Path.Combine(Settings.SolutionPath, "temp.h");

                File.WriteAllText(tempFile, function);

                foreach (var subrequirement in requirement.SubRequirements)
                {
                    if (await checker.CheckCorrectness(subrequirement.Title, functionMatch, tempFile, studReq))
                    {
                        studReq.SubRequirements.Last().Points = subrequirement.Points;
                        studReq.Points += studReq.SubRequirements.Last().Points;
                    }
                }

                student.Grade += studReq.Points;
            }
        }
    }
}