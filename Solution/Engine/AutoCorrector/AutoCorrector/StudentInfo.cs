using AutoCorrectorEngine;

namespace AutoCorrector;

public class StudentInfo
{
    public string Name { get; set; }
    public float Grade { get; set; }
    public bool CodeCompiles { get; set; }
    public string? SourceFile { get; set; }
    public List<Requirement> Requirements { get; set; } = new List<Requirement>();
}