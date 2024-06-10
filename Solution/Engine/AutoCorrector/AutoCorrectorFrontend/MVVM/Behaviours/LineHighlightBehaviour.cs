using AutoCorrectorFrontend.MVVM.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.Xaml.Behaviors;


namespace AutoCorrectorFrontend.MVVM.Behaviours
{
    public class LineHighlightBehavior : Behavior<TextEditor>
    {
        public static readonly DependencyProperty LineHighlightsProperty =
            DependencyProperty.Register("LineHighlights", typeof(ObservableCollection<ComparisonViewModel.LineHighlight>), typeof(LineHighlightBehavior), new PropertyMetadata(null, OnLineHighlightsChanged));

        public ObservableCollection<ComparisonViewModel.LineHighlight> LineHighlights
        {
            get { return (ObservableCollection<ComparisonViewModel.LineHighlight>)GetValue(LineHighlightsProperty); }
            set { SetValue(LineHighlightsProperty, value); }
        }

        private static void OnLineHighlightsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (LineHighlightBehavior)d;
            if (e.OldValue is ObservableCollection<ComparisonViewModel.LineHighlight> oldCollection)
            {
                oldCollection.CollectionChanged -= behavior.OnLineHighlightsCollectionChanged;
            }

            if (e.NewValue is ObservableCollection<ComparisonViewModel.LineHighlight> newCollection)
            {
                newCollection.CollectionChanged += behavior.OnLineHighlightsCollectionChanged;
            }

            behavior.UpdateLineTransformers();
        }

        private void OnLineHighlightsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateLineTransformers();
        }

        private readonly List<LineColorizer> lineColorizers = new List<LineColorizer>();

        private void UpdateLineTransformers()
        {
            if (AssociatedObject == null || LineHighlights == null)
            {
                return;
            }

            var textView = AssociatedObject.TextArea.TextView;

            // Remove existing line highlight transformers
            foreach (var colorizer in lineColorizers)
            {
                textView.LineTransformers.Remove(colorizer);
            }
            lineColorizers.Clear();

            // Add new line highlight transformers
            foreach (var highlight in LineHighlights)
            {
                var colorizer = new LineColorizer(highlight.StartLine, highlight.EndLine, new SolidColorBrush(highlight.HighlightColor));
                textView.LineTransformers.Add(colorizer);
                lineColorizers.Add(colorizer);
            }

            textView.InvalidateVisual();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (LineHighlights != null)
            {
                LineHighlights.CollectionChanged += OnLineHighlightsCollectionChanged;
            }
            UpdateLineTransformers();
        }

        protected override void OnDetaching()
        {
            if (LineHighlights != null)
            {
                LineHighlights.CollectionChanged -= OnLineHighlightsCollectionChanged;
            }
            base.OnDetaching();
        }
    }

    public class LineColorizer : DocumentColorizingTransformer
    {
        private readonly int startLine;
        private readonly int endLine;
        private readonly Brush brush;

        public LineColorizer(int startLine, int endLine, Brush brush)
        {
            this.startLine = startLine;
            this.endLine = endLine;
            this.brush = brush;
        }

        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
        {
            if (line.LineNumber >= startLine && line.LineNumber <= endLine)
            {
                ChangeLinePart(
                    line.Offset,
                    line.EndOffset,
                    element => element.TextRunProperties.SetBackgroundBrush(brush)
                );
            }
        }
    }


}