using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.IO;
using AutoCorrectorFrontend.MVVM.ViewModel;

namespace AutoCorrectorFrontend.MVVM.View
{
    /// <summary>
    /// Interaction logic for ScaleCreatorView.xaml
    /// </summary>
    public partial class ScaleCreatorView : UserControl
    {
        public ScaleCreatorView()
        {
            InitializeComponent();
        }

        private void DocDrop(object sender, DragEventArgs e)
        {
            ScaleCreatorViewModel homeViewModel = DataContext as ScaleCreatorViewModel;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    if (System.IO.Path.GetExtension(file).Equals(".pdf", System.StringComparison.OrdinalIgnoreCase) ||
                        System.IO.Path.GetExtension(file).Equals(".docx", System.StringComparison.OrdinalIgnoreCase) ||
                        System.IO.Path.GetExtension(file).Equals(".txt", System.StringComparison.OrdinalIgnoreCase))
                    {
                        homeViewModel.UploadedDocument = file;
                    }
                    else
                    {
                        MessageBox.Show("Please drop only .pdf .docx and .txt files.");
                    }
                }
            }
        }
    }
}
