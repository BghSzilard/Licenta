using AutoCorrectorEngine;
using AutoCorrectorFrontend.MVVM.Services;

namespace AutoCorrector;
public class StudentManager
{
    private readonly FileProcessor _fileProcessor;
    private readonly List<StudentInfo> _students;
    private readonly NotificationService _notificationService;
    private string _scale { get; set; }
    public StudentManager(NotificationService notificationService, string uploadedZip, string scale) 
    {
        _fileProcessor = new FileProcessor();
        _students = new List<StudentInfo>();
        _notificationService = notificationService;
        _scale = scale;
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
    public async Task GradeStudents(List<Requirement> processedScalde)
    {
        _notificationService.NotificationText = "Grading Students...";

        foreach (var student in _students)
        {
            int task = 1;

            if (!student.CodeCompiles)
            {
                foreach (var req in processedScalde)
                {
                    Requirement requirement = new Requirement();

                    foreach (var subReq in req.SubRequirements)
                    {
                        requirement.SubRequirements.Add(new SubRequirement());
                    }

                    student.Requirements.Add(requirement);
                }
            }
            else
            {
                foreach (var requirement in processedScalde)
                {
                    int subtask = 1;

                    FunctionSignatureExtractorWrapper functionSignatureExtractor = new FunctionSignatureExtractorWrapper();
                    var signatures = functionSignatureExtractor.GetSignatures(student.SourceFile);

                    string allSignatures = "";
                    foreach (var signature in signatures)
                    {
                        allSignatures += signature;
                        allSignatures += ";";
                    }
                    allSignatures = allSignatures.Remove(allSignatures.Length - 1);

                    LLMManager lLMManager = new LLMManager();
                    var functionName = await lLMManager.GetFunctionName($"{allSignatures} \n{requirement.Title}");

                    if (functionName.Contains("None"))
                    {
                        return;
                    }

                    FunctionExtractorWrapper functionExtractor = new FunctionExtractorWrapper();

                    FileProcessor fileProcessor = new FileProcessor();
                    var includes = fileProcessor.FindIncludes(student.SourceFile!);
                    var function = includes;
                    var extractedFunction = functionExtractor.GetFunction(student.SourceFile, functionName);
                    function += extractedFunction;

                    Requirement studReq = new Requirement();
                    studReq.Title = functionName;

                    student.Requirements.Add(studReq);

                    CorrectionChecker checker = new CorrectionChecker();


                    foreach (var subrequirement in requirement.SubRequirements)
                    {
                        _notificationService.NotificationText = $"Grading {student.Name} Task {task}.{subtask}";
                        var result = await checker.CheckCorrectness(function, subrequirement.Title, functionName);
                        SubRequirement subStudReq = new SubRequirement();

                        if (result.Contains("Yes:") || result.Contains("Success!"))
                        {
                            subStudReq.Title = result.Replace("Yes:", "");
                            subStudReq.Title = subStudReq.Title.Replace("Success!:", "");
                            subStudReq.Points = subrequirement.Points;
                            studReq.Points += subStudReq.Points;
                        }
                        else
                        {
                            subStudReq.Title = result.Replace("No:", "");
                            subStudReq.Title = subStudReq.Title.Replace("Fail!", "");
                        }
                        studReq.SubRequirements.Add(subStudReq);

                        subtask++;
                    }

                    student.Grade += studReq.Points;

                    task++;
                }

            }
            
        }
        Settings.StudentSample = _students[0];
        Settings.Students = _students;

        PlagiarismChecker plagiarismChecker = new PlagiarismChecker();

        Settings.PlagiarismPairs = await plagiarismChecker.CheckPlagiarism(_students);
    }
}