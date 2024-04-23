using System.Collections;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using AutoCorrector;
using AutoCorrectorEngine;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OfficeOpenXml.FormulaParsing.Ranges;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class ResultsViewModel: ObservableObject
{
    [ObservableProperty]
    private string _selectedReason;

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