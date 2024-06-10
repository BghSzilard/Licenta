using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using AutoCorrectorFrontend.MVVM.Converters;
using AutoCorrectorFrontend.MVVM.Model;
using AutoCorrectorFrontend.MVVM.ViewModel;
using DocumentFormat.OpenXml.Bibliography;

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
                rightCode.Text = comparisonViewModel.RightTextViewText;

                CreateSquares();
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

        private Border _selectedBorder; // Field to keep track of the selected Border

        private void CreateSquares()
        {
            // Create WrapPanel
            var wrapPanel = new WrapPanel
            {
                Margin = new Thickness(10)
            };

            // Set Grid.Row for the WrapPanel
            Grid.SetRow(wrapPanel, 2);

            // Add WrapPanel to the Grid
            LeftGrid.Children.Add(wrapPanel);

            // Get the squares from the ViewModel
            var squares = ((ComparisonViewModel)this.DataContext).Squares1;
            bool isFirst = true;
            // Create and add Border elements to the WrapPanel
            foreach (var square in squares)
            {
                if (square.Number == 1)
                {
                    square.IsSelected = true;
                }

                var border = new Border
                {
                    Width = 50,
                    Height = 50,
                    Margin = new Thickness(5),
                    BorderBrush = Brushes.White,
                    BorderThickness = new Thickness(2),
                    Background = isFirst ? new SolidColorBrush(Color.FromArgb(255, 135, 206, 250)) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4287f5")),
                    DataContext = square
                };

                if (isFirst)
                {
                    _selectedBorder = border;
                    isFirst = false;
                    ((ComparisonViewModel)this.DataContext).SelectedFile1Id = square.Number;

                }

                // Add MouseLeftButtonUp Event Handler
                border.MouseLeftButtonUp += Square_Click;

                // Create TextBlock for the number
                var textBlock = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.White,
                    FontSize = 20
                };
                textBlock.SetBinding(TextBlock.TextProperty, new Binding("Number"));

                border.MouseEnter += Border_MouseEnter;
                border.MouseLeave += Border_MouseLeave;



                // Add TextBlock to Border
                border.Child = textBlock;

                // Add Border to WrapPanel
                wrapPanel.Children.Add(border);
            }
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null && border != _selectedBorder)
            {
                border.Cursor = Cursors.Hand; // Change cursor to hand
                border.Background = new SolidColorBrush(Color.FromArgb(255, 135, 206, 250)); // Change to a glowing color
            }

        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null && border != _selectedBorder)
            {
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4287f5")); // Revert to the standard color
            }

        }

        private void Square_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                if (_selectedBorder != null)
                {
                    _selectedBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4287f5")); // Deselect the previously selected square
                }
                border.Background = new SolidColorBrush(Color.FromArgb(255, 135, 206, 250));
                _selectedBorder = border; // Update the selected square

                // Retrieve the number from the square's DataContext
                var square = border.DataContext as Square;
                if (square != null)
                {
                    // Update the ViewModel with the number of the selected square
                    ((ComparisonViewModel)this.DataContext).SelectedFile1Id = square.Number;
                }
                leftCode.Text = ((ComparisonViewModel)this.DataContext).LeftTextViewText;
            }

        }
    }
}