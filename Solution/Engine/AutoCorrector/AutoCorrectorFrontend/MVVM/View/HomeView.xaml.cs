using System.IO;
using System.Windows;
using System.Windows.Controls;

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
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    if (Path.GetExtension(file).Equals(".txt", System.StringComparison.OrdinalIgnoreCase))
                    {
                        // Process dropped text file here
                        MessageBox.Show("Dropped text file: " + file);
                    }
                    else
                    {
                        // Not a text file, handle accordingly
                        MessageBox.Show("Please drop only text files.");
                    }
                }
            }
        }


        private void ProjectsDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    if (Path.GetExtension(file).Equals(".txt", System.StringComparison.OrdinalIgnoreCase))
                    {
                        // Process dropped text file here
                        MessageBox.Show("Dropped project: " + file);
                    }
                    else
                    {
                        // Not a text file, handle accordingly
                        MessageBox.Show("Please drop only text files.");
                    }
                }
            }
        }
    }
}
