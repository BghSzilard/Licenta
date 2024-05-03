using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AutoCorrectorFrontend.MVVM.Model;
using AutoCorrectorFrontend.MVVM.ViewModel;

namespace AutoCorrectorFrontend.MVVM.View
{
    public partial class ComparisonView: UserControl
    {
        public ComparisonView()
        {
            InitializeComponent();

            DataContextChanged += ComparisonView_DataContextChanged;
        }

        private void ComparisonView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ComparisonViewModel comparisonViewModel)
            {
                leftCode.Text = comparisonViewModel.LeftTextViewText;
                leftCode.TextArea.TextView.LineTransformers.Add(new LineColorizer(0, 5, new SolidColorBrush(Colors.Yellow)));
                rightCode.Text = comparisonViewModel.RightTextViewText;
            }
        }
        private void leftTextView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var viewModel = DataContext as ComparisonViewModel;
            if (viewModel != null && viewModel.ChangeLeftFontSizeCommand.CanExecute(e))
            {
                viewModel.ChangeLeftFontSizeCommand.Execute(e);
            }
        }

        private void rightTextView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var viewModel = DataContext as ComparisonViewModel;
            if (viewModel != null && viewModel.ChangeRightFontSizeCommand.CanExecute(e))
            {
                viewModel.ChangeRightFontSizeCommand.Execute(e);
            }
        }
    }
}