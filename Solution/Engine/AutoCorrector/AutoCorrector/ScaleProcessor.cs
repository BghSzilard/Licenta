using System.Text;
using System.Xml.Linq;
using AutoCorrectorFrontend.MVVM.Services;

namespace AutoCorrectorEngine;

public class ScaleProcessor
{
    private NotificationService _notificationService;
    private void SaveProcessedScale(List<Requirement> requirements)
    {
        StringBuilder scale = new StringBuilder();
        foreach (var req in requirements)
        {
            scale.AppendLine(req.Title);
            foreach (var subreq in req.SubRequirements)
            {
                scale.AppendLine($"-{subreq.Title}");
            }
        }

        string result = scale.ToString();
        File.WriteAllText("C:\\Users\\z004w26z\\Desktop\\processedBarem.txt", result);

    }
    public ScaleProcessor(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    public async Task<List<Requirement>> ProcessScale(string path)
    {
        Translate translate = new Translate();

        var requirements = ReadXMLFile(path);
        List<Requirement> processedScale = new List<Requirement>();
        foreach (var requirement in requirements)
        {
            Requirement processedRequirement = new Requirement();
            _notificationService.NotificationText = $"Processing Requirement: {requirement.Title}";
            //processedRequirement.Title = await GetFunctionSignature(requirement.Title);
            processedRequirement.Title = await translate.TranslateToEnglish(requirement.Title);
            foreach (var subrequirement in requirement.SubRequirements)
            {
                //SubRequirement processedSubrequirement = new SubRequirement();
                //processedSubrequirement.Points = subrequirement.Points;
                //_notificationService.NotificationText = $"Processing Subrequirement: {subrequirement.Title}";
                //processedSubrequirement.Title = await ProcessSubtask(subrequirement.Title);
                var processedSubrequirement = subrequirement;
                processedSubrequirement.Title = await translate.TranslateToEnglish(subrequirement.Title);
                processedSubrequirement.Title = await ProcessSubtask(subrequirement.Title);
                processedRequirement.SubRequirements.Add(processedSubrequirement);
            }
            processedScale.Add(processedRequirement);
        }

        SaveProcessedScale(processedScale);


        return processedScale;
    }
    private async Task<string> ProcessSubtask(string subrequirement)
    {
        LLMManager lLMManager = new LLMManager();
        return await lLMManager.ProcessSubtask(subrequirement);
    }
   
    private List<Requirement> ReadXMLFile(string filePath)
    {
        List<Requirement> requirements = new List<Requirement>();

        try
        {
            XDocument doc = XDocument.Load(filePath);

            requirements = doc.Descendants("task").Select(taskElement => new Requirement
            {
                Title = taskElement.Element("title")?.Value,
                Points = float.Parse(taskElement.Element("points")?.Value ?? "0"),
                SubRequirements = taskElement.Descendants("subtask").Select(subtaskElement => new SubRequirement
                {
                    Title = subtaskElement.Element("title")?.Value,
                    Points = float.Parse(subtaskElement.Element("points")?.Value ?? "0")
                }).ToList()
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing XML file: {ex.Message}");
        }

        return requirements;
    }
}