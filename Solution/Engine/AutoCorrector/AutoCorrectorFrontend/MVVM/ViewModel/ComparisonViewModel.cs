using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using AutoCorrectorEngine;
using AutoCorrectorFrontend.MVVM.Model;
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

    public ObservableCollection<Square> Squares1 = new ObservableCollection<Square>();
    public ObservableCollection<Square> Squares2 = new ObservableCollection<Square>();

    public ObservableCollection<LineHighlight> LineHighlights1 { get; set; } = new ObservableCollection<LineHighlight> ();
    public ObservableCollection<LineHighlight> LineHighlights2 { get; set; } = new ObservableCollection<LineHighlight> ();

    [ObservableProperty]
    private IHighlightingDefinition _highlightingDefinition;

    [ObservableProperty]
    private int _selectedFile1Id;

    [ObservableProperty]
    private int _selectedFile2Id;

    private PlagiarismPair _plagiarismPair = new PlagiarismPair();

    private List<Color> _colors = new List<Color>();
    partial void OnSelectedFile1IdChanged(int oldValue, int newValue)
    {
        LeftTextViewText = _plagiarismPair.Files1.Where(x => x.Id == newValue).ToList()[0].Content;
        SelectedFile1Id = newValue;
        LineHighlights1.Clear();
        LineHighlights2.Clear();
        UpdateHighlights();
    }

    partial void OnSelectedFile2IdChanged(int oldValue, int newValue)
    {
        RightTextViewText = _plagiarismPair.Files2.Where(x => x.Id == newValue).ToList()[0].Content;
        SelectedFile2Id = newValue;
        LineHighlights1.Clear();
        LineHighlights2.Clear();
        UpdateHighlights();
    }

    private void UpdateHighlights()
    {
        int temp = 0;

        foreach (var match in _plagiarismPair.matches)
        {
            if (match.File1.Id == SelectedFile1Id)
            {
                LineHighlights1.Add(new LineHighlight() { StartLine = match.Start1, EndLine = match.End1, HighlightColor = _colors[temp] });
            }

            if (match.File2.Id == SelectedFile2Id)
            {
                LineHighlights2.Add(new LineHighlight() { StartLine = match.Start2, EndLine = match.End2, HighlightColor = _colors[temp] });
            }

            temp++;
        }
    }

    public ComparisonViewModel(PlagiarismPair plagiarismPair)
    {
        _plagiarismPair = plagiarismPair;

        _colors = GenerateRandomColors(plagiarismPair.matches.Count);

        SelectedFile1Id = 1;
        SelectedFile2Id = 1;

        for (int i = 0; i < plagiarismPair.Files1.Count; i++)
        {
            Squares1.Add(new Square { Number = i + 1 });
        }

        for (int i = 0; i < plagiarismPair.Files2.Count; i++)
        {
            Squares2.Add(new Square { Number = i + 1});
        }

        LeftTextViewText = plagiarismPair.Files1.Where(x => x.Id == 1).ToList()[0].Content;
        RightTextViewText = plagiarismPair.Files2.Where(x => x.Id == 1).ToList()[0].Content;

        FirstName = plagiarismPair.Id1;
        SecondName = plagiarismPair.Id2;

        using (var reader = new StreamReader(Settings.SyntaxPath))
        {
            HighlightingDefinition = HighlightingLoader.Load(new XmlTextReader(reader), HighlightingManager.Instance);
        }


       UpdateHighlights();
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