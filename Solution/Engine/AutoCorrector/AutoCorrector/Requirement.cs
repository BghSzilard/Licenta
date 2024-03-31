namespace AutoCorrectorEngine;

public class Requirement
{
    public string Title { get; set; }
    public List<SubRequirement> SubRequirements { get; set; }
    public float Points { get; set; }
    public Requirement()
    {
        Title = string.Empty;
        SubRequirements = new List<SubRequirement>();
    }
}