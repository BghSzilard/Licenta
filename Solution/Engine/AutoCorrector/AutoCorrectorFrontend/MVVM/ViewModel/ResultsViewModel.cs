using System.IO;
using System.Windows.Controls;
using AutoCorrector;
using AutoCorrectorEngine;
using AutoCorrectorFrontend.MVVM.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCorrectorFrontend.MVVM.ViewModel;
public partial class ResultsViewModel : ObservableObject
{
    private NotificationService _notificationService;
    public ResultsViewModel(NotificationService notificationService)
    {
        foreach (var stud in Settings.Students)
        {
            _students.Add(stud);
        }

        _notificationService = notificationService;
    }
    
    [ObservableProperty]
    private string? _selectedReason;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ResultsReady))]
    private List<StudentInfo> _students = new List<StudentInfo>();
    [RelayCommand]
    public void SelectedCellsChanged(DataGridCellInfo dataGridCellInfo)
    {
        var column = dataGridCellInfo.Column;

        if (column != null &&  column.Header != null && column.Header.ToString().Contains("Task") && column.Header.ToString().Contains("."))
        {
            StudentInfo student = dataGridCellInfo.Item as StudentInfo;
            var requirement = GetNthDigit(column.Header.ToString(), 1);
            var subReq = GetNthDigit(column.Header.ToString(), 2);

            // Ensure the requirement index is within bounds
            if (int.TryParse(requirement.ToString(), out int reqIndex) && reqIndex > 0 && reqIndex <= student.Requirements.Count)
            {
                var requirementItem = student.Requirements[reqIndex - 1];

                // Ensure the subrequirement index is within bounds
                if (int.TryParse(subReq.ToString(), out int subReqIndex) && subReqIndex > 0 && subReqIndex <= requirementItem.SubRequirements.Count)
                {
                    SelectedReason = requirementItem.SubRequirements[subReqIndex - 1].Title;
                }
                else
                {
                    // Handle out of bounds subrequirement index
                    Console.WriteLine("Subrequirement index is out of bounds.");
                }
            }
            else
            {
                // Handle out of bounds requirement index
                Console.WriteLine("Requirement index is out of bounds.");
            }


        }
    }

    [RelayCommand]
    public void EditSelectedCell(DataGridCellInfo dataGridCellInfo)
    {
        var column = dataGridCellInfo.Column;

        if (column.Header.ToString().Contains("Task") && column.Header.ToString().Contains('.'))
        {
            StudentInfo stud = dataGridCellInfo.Item as StudentInfo;
            var studToEdit = Students.FirstOrDefault(x => x.Name == stud.Name);

            var requirement = GetNthDigit(column.Header.ToString(), 1);
            var subReq = GetNthDigit(column.Header.ToString(), 2);

            DataGridCell cell = dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item).Parent as DataGridCell;

            string cellText = cell.Content.ToString();

            cellText = cellText.Replace("System.Windows.Controls.TextBox: ", "");

            studToEdit.Requirements[int.Parse(requirement.ToString()) - 1].SubRequirements[int.Parse(subReq.ToString()) - 1].Points = float.Parse(cellText);

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

        throw new InvalidOperationException($"No {n}th numerical character found in the input string.");
    }

    public bool ResultsReady => Students.Count > 0;

    [RelayCommand]
    public async Task SaveResults()
    {
        FileInfo fileInfo = new FileInfo(Settings.ResultsPath);
        ExcelManager excelManager = new ExcelManager();
        await excelManager.SaveExcelFile(Students);
        _notificationService.NotificationText = "Results Saved!";
    }
}