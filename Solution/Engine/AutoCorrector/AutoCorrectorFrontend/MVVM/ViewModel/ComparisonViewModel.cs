using System.IO;
using System.Windows.Input;
using System.Xml;
using AutoCorrectorEngine;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using static AutoCorrectorEngine.PlagiarismChecker;

namespace AutoCorrectorFrontend.MVVM.ViewModel;

public partial class ComparisonViewModel : ObservableObject
{
    [ObservableProperty]
    private IHighlightingDefinition _highlightingDefinition;
    public ComparisonViewModel(PlagiarismPair plagiarismPair)
    {
        LeftTextViewText = plagiarismPair.SourceFile1;
        RightTextViewText = plagiarismPair.SourceFile2;

        FirstName = plagiarismPair.Id1;
        SecondName = plagiarismPair.Id2;

        using (var reader = new StreamReader(Settings.SyntaxPath))
        {
            HighlightingDefinition = HighlightingLoader.Load(new XmlTextReader(reader), HighlightingManager.Instance);
        }
    }

    [ObservableProperty]
    private string _firstName;

    [ObservableProperty]
    private string _secondName;

    [ObservableProperty]
    private double _leftViewerFontSize = 12;

    [ObservableProperty]
    private double _rightViewerFontSize = 12;

    [ObservableProperty]
    private string _leftTextViewText;

    [ObservableProperty]
    private string _rightTextViewText;

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