using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoCorrectorFrontend.MVVM.ViewModel;

namespace AutoCorrectorFrontend.MVVM.View
{
    /// <summary>
    /// Interaction logic for ComparisonView.xaml
    /// </summary>
    public partial class ComparisonView : UserControl
    {
        public ComparisonView()
        {
            InitializeComponent();
        }

        private void leftTextView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var viewModel = DataContext as ComparisonViewModel;
            if (viewModel != null && viewModel.ChangeLeftFontSizeCommand.CanExecute(e.Delta))
            {
                viewModel.ChangeLeftFontSizeCommand.Execute(e.Delta);
            }
        }

        private void rightTextView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var viewModel = DataContext as ComparisonViewModel;
            if (viewModel != null && viewModel.ChangeRightFontSizeCommand.CanExecute(e.Delta))
            {
                viewModel.ChangeRightFontSizeCommand.Execute(e.Delta);
            }
        }
    }
}
