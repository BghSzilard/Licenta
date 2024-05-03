using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Media;

namespace AutoCorrectorFrontend.MVVM.Model;

public class LineColorizer : DocumentColorizingTransformer
{
    private readonly int _startLine;
    private readonly int _endLine;
    private readonly SolidColorBrush _highlightBrush;

    public LineColorizer(int startLine, int endLine, SolidColorBrush highlightBrush)
    {
        _startLine = startLine;
        _endLine = endLine;
        _highlightBrush = highlightBrush;
    }

    protected override void ColorizeLine(DocumentLine line)
    {
        if (line.LineNumber >= _startLine && line.LineNumber <= _endLine)
        {
            ChangeLinePart(
                line.Offset,
                line.EndOffset,
                element => element.BackgroundBrush = _highlightBrush);
        }
    }
}