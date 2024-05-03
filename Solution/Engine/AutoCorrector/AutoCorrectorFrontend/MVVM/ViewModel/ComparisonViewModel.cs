using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
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
    public class LineHighlight: ObservableObject
    {
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public Color HighlightColor { get; set; }
    }

    public ObservableCollection<LineHighlight> LineHighlights1 { get; set; } = new ObservableCollection<LineHighlight> ();
    public ObservableCollection<LineHighlight> LineHighlights2 { get; set; } = new ObservableCollection<LineHighlight> ();

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

        var colors = GenerateRandomColors(plagiarismPair.matches.Count);

        int temp = 0;

        foreach (var match in plagiarismPair.matches)
        {
            LineHighlights1.Add(new LineHighlight() { StartLine = match.Start1, EndLine = match.End1, HighlightColor = colors[temp] });
            LineHighlights2.Add(new LineHighlight() { StartLine = match.Start2, EndLine = match.End2, HighlightColor = colors[temp] });
            temp++;
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

    public List<Color> GenerateRandomColors(int n)
    {
        var random = new Random();
        var colors = new List<Color>
    {
        Colors.LightGreen, 
        Colors.Yellow, 
        Colors.Orange 
    };

        for (int i = 3; i < n; i++)
        {
            var color = Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
            colors.Add(color);
        }

        return colors;
    }
}