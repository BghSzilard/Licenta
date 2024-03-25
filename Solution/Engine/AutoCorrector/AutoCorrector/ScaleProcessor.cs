using System.Xml;
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
            processedRequirement.MainRequirement = GetFunctionSignature(requirement.MainRequirement);
            foreach (var subrequirement in requirement.SubRequirements)
            {
                processedRequirement.SubRequirements.Add(ProcessSubtask(subrequirement));
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
        List<Requirement> list = new List<Requirement>();
        // Specify the file path of the XML document
        string xmlFilePath = filePath;

        // Load the XML document from the file path
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlFilePath);

        // Get the root element
        XmlElement root = xmlDoc.DocumentElement;

        // Get a list of all task nodes
        XmlNodeList taskNodes = root.SelectNodes("//task");

        // Iterate through each task node
        foreach (XmlNode taskNode in taskNodes)
        {
            string taskName = taskNode.SelectSingleNode("main_task").InnerText;
            Requirement requirement = new Requirement();
            requirement.MainRequirement = taskName;

            // Get a list of subtask nodes
            XmlNodeList subtaskNodes = taskNode.SelectNodes("subtasks/subtask");

            // Iterate through each subtask node
            foreach (XmlNode subtaskNode in subtaskNodes)
            {
                string subtaskName = subtaskNode.InnerText;
                requirement.SubRequirements.Add(subtaskName);
            }

            list.Add(requirement);
        }

        return list;
    }
}