using System.Xml.Linq;
using AutoCorrectorFrontend.MVVM.Services;

namespace AutoCorrectorEngine;

public class ScaleProcessor
{
    private NotificationService _notificationService;
    public ScaleProcessor(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    public async Task<List<Requirement>> ProcessScale(string path)
    {
        var now = DateTime.Now;
        var requirements = ReadXMLFile(path);
        _notificationService.NotificationText = "Scale Read!";
        List<Requirement> processedScale = new List<Requirement>();
        foreach (var requirement in requirements)
        {
            Requirement processedRequirement = new Requirement();
            processedRequirement.Title = await GetFunctionSignature(requirement.Title);
            _notificationService.NotificationText = $"{requirement.Title} processed!";
            foreach (var subrequirement in requirement.SubRequirements)
            {
                SubRequirement processedSubrequirement = new SubRequirement();
                processedSubrequirement.Points = subrequirement.Points;
                processedSubrequirement.Title = await ProcessSubtask(subrequirement.Title);
                processedRequirement.SubRequirements.Add(processedSubrequirement);
                _notificationService.NotificationText = $"{subrequirement.Title} processed!";
            }
            processedScale.Add(processedRequirement);
        }


        var elapsedTime = DateTime.Now - now;
        Console.WriteLine(elapsedTime.ToString());

        return processedScale;
    }
    private async Task<string> ProcessSubtask(string subrequirement)
    {
        LLMManager lLMManager = new LLMManager();
        return await lLMManager.ProcessSubtask(subrequirement);
    }
    private async Task<string> GetFunctionSignature(string requirement)
    {
        LLMManager lLMManager = new LLMManager();
        string functionSignature = "\"";

        functionSignature += await lLMManager.GetFunctionSignature(requirement);
        functionSignature = functionSignature.Replace("\n", "");
        functionSignature = functionSignature.Replace("\r", "");
        functionSignature += "\"";

        return functionSignature;
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