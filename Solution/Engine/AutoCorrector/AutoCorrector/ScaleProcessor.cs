using System.Xml;
using System.Xml.Linq;
using AutoCorrector;

namespace AutoCorrectorEngine;

public class ScaleProcessor
{
    public List<Requirement> ProcessScale(string path)
    {
        var now = DateTime.Now;
        var requirements = ReadXMLFile(path);
        List<Requirement> processedScale = new List<Requirement>();
        foreach (var requirement in requirements)
        {
            Requirement processedRequirement = new Requirement();
            processedRequirement.Title = GetFunctionSignature(requirement.Title);
            foreach (var subrequirement in requirement.SubRequirements)
            { 
                processedRequirement.SubRequirements.Add(subrequirement);
            }
            processedScale.Add(processedRequirement);
        }

        
        var elapsedTime = DateTime.Now - now;
        Console.WriteLine(elapsedTime.ToString());

        return processedScale;
    }
    private string ProcessSubtask(string subrequirement)
    {
        LLMManager lLMManager = new LLMManager();
        return lLMManager.ProcessSubtask(subrequirement);
    }
    private string GetFunctionSignature(string requirement)
    {
        LLMManager lLMManager = new LLMManager();
        string functionSignature = "\"";

        functionSignature += lLMManager.GetFunctionSignature(requirement);
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