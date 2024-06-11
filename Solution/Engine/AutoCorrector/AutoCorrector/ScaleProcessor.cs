using System.Text;
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
        
        var requirements = await ReadXMLFile(path);
        List<Requirement> processedScale = new List<Requirement>();
        foreach (var requirement in requirements)
        {
            Requirement processedRequirement = new Requirement();
            _notificationService.NotificationText = $"Processing Requirement: {requirement.Title}";
            
            processedRequirement.Title = requirement.Title;
            processedRequirement.Type = requirement.Type;
            foreach (var subrequirement in requirement.SubRequirements)
            {
                
                var processedSubrequirement = subrequirement;
                
                processedRequirement.SubRequirements.Add(processedSubrequirement);
            }
            processedScale.Add(processedRequirement);
        }

        return processedScale;
    }
    private async Task<List<Requirement>> ReadXMLFile(string filePath)
    {
        List<Requirement> requirements = new List<Requirement>();

        try
        {
            requirements = await Task.Run(() =>
            {
                XDocument doc = XDocument.Load(filePath);

                return doc.Descendants("task").Select(taskElement => new Requirement
                {
                    Title = taskElement.Element("title")?.Value,
                    Type = taskElement.Element("type")?.Value,
                    Points = float.Parse(taskElement.Element("points")?.Value ?? "0"),
                    SubRequirements = taskElement.Descendants("subtask").Select(subtaskElement => new SubRequirement
                    {
                        Title = subtaskElement.Element("title")?.Value,
                        Type = subtaskElement.Element("type")?.Value,
                        Points = float.Parse(subtaskElement.Element("points")?.Value ?? "0")
                    }).ToObservableCollection()
                }).ToList();
            });

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing XML file: {ex.Message}");
        }

        return requirements;
    }
}