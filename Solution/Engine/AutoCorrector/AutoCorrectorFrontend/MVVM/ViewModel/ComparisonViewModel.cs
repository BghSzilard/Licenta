using System.Collections.ObjectModel;
using System.Windows.Input;
using AutoCorrectorEngine;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class ComparisonViewModel : ObservableObject
{
    public ComparisonViewModel(string leftText, string rightText)
    {
        LeftTextViewText = leftText;
        RightTextViewText = rightText;

        LeftLineNumbers = GenerateLineNumbers(leftText).ToObservableCollection();
        RightLineNumbers = GenerateLineNumbers(rightText).ToObservableCollection();
    }

    [ObservableProperty]
    private double _leftViewerFontSize = 12;

    [ObservableProperty]
    private double _rightViewerFontSize = 12;

    [ObservableProperty]
    private string _leftTextViewText;

    [ObservableProperty]
    private string _rightTextViewText;

    public ObservableCollection<string> LeftLineNumbers { get; set; }
    public ObservableCollection<string> RightLineNumbers { get; set; }

    private List<string> GenerateLineNumbers(string text)
    {
        List<string> lineNumbersList = new List<string>();
        for (int i = 1; i <= text.Split('\n').Length; i++)
        {
            lineNumbersList.Add(i.ToString());
        }
        return lineNumbersList;
    }

    [RelayCommand]
    public void ChangeLeftFontSize(MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            if (e.Delta > 0)
            {    
                LeftViewerFontSize *= 1.1;
            }
            else
            {
                LeftViewerFontSize *= 0.9;
            }
        }
    }

    [RelayCommand]
    public void ChangeRightFontSize(MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            if (e.Delta > 0)
            {
                RightViewerFontSize *= 1.1;
            }
            else
            {
                RightViewerFontSize *= 0.9;
            }
        }
    }
}