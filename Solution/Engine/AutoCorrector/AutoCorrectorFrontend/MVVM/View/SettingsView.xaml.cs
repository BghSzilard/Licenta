using System.Windows.Controls;

namespace AutoCorrectorFrontend.MVVM.View
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();

            SettingRectangle.MouseLeftButtonDown += (s, e) => ThresholdTextBox.Focus();
        }
    }
}
