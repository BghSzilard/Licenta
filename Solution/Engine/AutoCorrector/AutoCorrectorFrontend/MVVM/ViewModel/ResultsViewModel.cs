using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using AutoCorrector;
using AutoCorrectorEngine;
using AutoCorrectorFrontend.MVVM.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Spreadsheet;
using static Microsoft.Win32.NativeMethods;

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
        else
        {
            SelectedReason = "";
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

            ContentPresenter contentPresenter = cell.Content as ContentPresenter;

            string cellText = VisualTreeHelper.GetChild(contentPresenter, 0).ToString();

            cellText = cellText.Replace("System.Windows.Controls.TextBox: ", "");

            studToEdit.Requirements[int.Parse(requirement.ToString()) - 1].SubRequirements[int.Parse(subReq.ToString()) - 1].Points = float.Parse(cellText);

            studToEdit.Requirements[int.Parse(requirement.ToString()) - 1].Points = 0;

            foreach (var req in studToEdit.Requirements[int.Parse(requirement.ToString()) -1].SubRequirements)
            {
                studToEdit.Requirements[int.Parse(requirement.ToString()) - 1].Points += req.Points;
            }

            studToEdit.Grade = 1;

            foreach (var req in studToEdit.Requirements)
            {
                studToEdit.Grade += req.Points;
            }
        }
    }

    [RelayCommand]
    public async void DoubleClick(DataGridCellInfo dataGridCellInfo)
    {
        var column = dataGridCellInfo.Column;
        ProcessExecutor processExecutor = new ProcessExecutor();
        
        if (column != null && column.Header.ToString().Contains("function"))
        {
            var requirement = GetNthDigit(column.Header.ToString(), 1);

            StudentInfo stud = dataGridCellInfo.Item as StudentInfo;

            int lastIndex = stud.SourceFile!.LastIndexOf('\\');

            
            string firstPart = stud.SourceFile!.Substring(0, lastIndex);
            string secondPart = stud.SourceFile!.Substring(lastIndex + 1);

            Process process = new Process();

            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.WorkingDirectory = firstPart;
            process.StartInfo.Arguments = $"code -g \"\"\"{stud.SourceFile}:{stud.Requirements[int.Parse(requirement.ToString()) - 1].Line}\"\"\"";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            await process.WaitForExitAsync();

            
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