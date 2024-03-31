using System.IO;
using System.Windows;
using System.Windows.Controls;
using AutoCorrectorFrontend.MVVM.ViewModel;

namespace AutoCorrectorFrontend.MVVM.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
           
        }

        private void ScaleDrop(object sender, DragEventArgs e)
        {
            HomeViewModel homeViewModel = DataContext as HomeViewModel;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    if (Path.GetExtension(file).Equals(".xml", System.StringComparison.OrdinalIgnoreCase))
                    {
                        homeViewModel.UploadedScale = file;
                    }
                    else
                    {
                        MessageBox.Show("Please drop only .xml files.");
                    }
                }
            }
        }


        private void ProjectsDrop(object sender, DragEventArgs e)
        {
            HomeViewModel homeViewModel = DataContext as HomeViewModel;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    if (Path.GetExtension(file).Equals(".txt", System.StringComparison.OrdinalIgnoreCase))
                    {
                        homeViewModel.UploadedZip = file;
                    }
                    else
                    {
                        // Not a text file, handle accordingly
                        MessageBox.Show("Please drop only zip files.");
                    }
                }
            }
        }
    }
}
