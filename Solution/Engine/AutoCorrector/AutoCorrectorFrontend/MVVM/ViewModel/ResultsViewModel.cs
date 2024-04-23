using System.Windows.Controls;
using AutoCorrector;
using AutoCorrectorEngine;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class ResultsViewModel : ObservableObject
{
    public ResultsViewModel()
    {
        foreach (var stud in Settings.Students)
        {
            _students.Add(stud);
        }

        foreach (var pair in Settings.plagiarismPairs)
        {
            PlagiarismPairs.Add(pair);
        }
    }
    [ObservableProperty]
    private List<PlagiarismPair> _plagiarismPairs = new List<PlagiarismPair>();
    [ObservableProperty]
    private string? _selectedReason;
    [ObservableProperty]
    private List<StudentInfo> _students = new List<StudentInfo>();
    [RelayCommand]
    public void SelectedCellsChanged(DataGridCellInfo dataGridCellInfo)
    {
        var column = dataGridCellInfo.Column;

        if (column.Header.ToString().Contains("Task") && column.Header.ToString().Contains("."))
        {
            StudentInfo student = dataGridCellInfo.Item as StudentInfo;
            var requirement = GetNthDigit(column.Header.ToString(), 1);
            var subReq = GetNthDigit(column.Header.ToString(), 2);
            SelectedReason = student.Requirements[int.Parse(requirement.ToString()) - 1].SubRequirements[int.Parse(subReq.ToString()) - 1].Title;
        }
    }

    [RelayCommand]
    public void EditSelectedCell(DataGridCellInfo dataGridCellInfo)
    {
        var column = dataGridCellInfo.Column;

        if (column.Header.ToString().Contains("Task") && column.Header.ToString().Contains("."))
        {
            StudentInfo stud = dataGridCellInfo.Item as StudentInfo;
            var studToEdit = Students.FirstOrDefault(x => x.Name == stud.Name);

            var requirement = GetNthDigit(column.Header.ToString(), 1);
            var subReq = GetNthDigit(column.Header.ToString(), 2);

            studToEdit.Requirements[int.Parse(requirement.ToString()) - 1].SubRequirements[int.Parse(subReq.ToString()) - 1].Points = stud.Requirements[int.Parse(requirement.ToString()) - 1].SubRequirements[int.Parse(subReq.ToString()) - 1].Points;

            studToEdit.Requirements[int.Parse(requirement.ToString()) - 1].Points = 0;

            foreach (var req in studToEdit.Requirements[int.Parse(requirement.ToString()) -1].SubRequirements)
            {
                studToEdit.Requirements[int.Parse(requirement.ToString()) - 1].Points += req.Points;
            }

            studToEdit.Grade = 0;

            foreach (var req in studToEdit.Requirements)
            {
                studToEdit.Grade += req.Points;
            }
        }
    }

    private char GetNthDigit(string input, int n)
    {
        int count = 0;
        foreach (char c in input)
        {
            if (char.IsDigit(c))
            {
                count++;
                if (count == n)
                {
                    return c;
                }
            }
        }
        // If there are fewer than n numerical characters in the input string, you can return a default value or throw an exception.
        throw new InvalidOperationException($"No {n}th numerical character found in the input string.");
    }
}