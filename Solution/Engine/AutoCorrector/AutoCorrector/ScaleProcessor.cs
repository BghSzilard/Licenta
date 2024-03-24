using System.Xml;
using AutoCorrector;

namespace AutoCorrectorEngine;

public class ScaleProcessor
{
    public string GetFunctionSignature(string requirement)
    {
        LLMManager lLMManager = new LLMManager();
        string functionSignature = "\"";

        functionSignature += lLMManager.GetFunctionSignature(requirement);
        functionSignature = functionSignature.Replace("\n", "");
        functionSignature = functionSignature.Replace("\r", "");
        functionSignature += "\"";

        return functionSignature;
    }

    public List<Requirement> ReadXMLFile(string filePath)
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