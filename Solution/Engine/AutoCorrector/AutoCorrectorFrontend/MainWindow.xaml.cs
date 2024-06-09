using System.IO;
using System.Windows;
using System.Windows.Input;
using AutoCorrectorFrontend.MVVM.ViewModel;
using Caliburn.Micro;
using Microsoft.Extensions.DependencyInjection;

namespace AutoCorrectorFrontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainViewModel(((App)Application.Current).ServiceProvider.GetRequiredService<EventAggregator>());
            InitializeComponent();
        }
    }
}