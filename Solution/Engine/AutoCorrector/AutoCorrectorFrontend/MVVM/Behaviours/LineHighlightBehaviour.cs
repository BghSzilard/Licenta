using AutoCorrectorFrontend.MVVM.Model;
using ICSharpCode.AvalonEdit;
using Microsoft.Xaml.Behaviors;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;
using AutoCorrectorFrontend.MVVM.ViewModel;

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
            behavior.UpdateLineTransformers();
        }

        private void UpdateLineTransformers()
        {
            if (AssociatedObject != null && LineHighlights != null)
            {
                foreach (var highlight in LineHighlights)
                {
                    AssociatedObject.TextArea.TextView.LineTransformers.Add(new LineColorizer(highlight.StartLine, highlight.EndLine, new SolidColorBrush(highlight.HighlightColor)));
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateLineTransformers();
        }
    }

}