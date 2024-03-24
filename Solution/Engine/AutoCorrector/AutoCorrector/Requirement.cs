namespace AutoCorrectorEngine;

public class Requirement
{
    public string MainRequirement { get; set; }
    public List<string> SubRequirements { get; set; }

    public Requirement()
    {
        MainRequirement = string.Empty;
        SubRequirements = new List<string>();
    }
}