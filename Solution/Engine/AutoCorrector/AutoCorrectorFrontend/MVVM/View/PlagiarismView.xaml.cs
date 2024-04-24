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
using AutoCorrector;
using AutoCorrectorEngine;
using AutoCorrectorFrontend.Events;
using AutoCorrectorFrontend.MVVM.ViewModel;
using Caliburn.Micro;
using Microsoft.Extensions.DependencyInjection;

namespace AutoCorrectorFrontend.MVVM.View
{
    /// <summary>
    /// Interaction logic for PlagiarismView.xaml
    /// </summary>
    public partial class PlagiarismView : UserControl
    {
        public PlagiarismView()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var row = (AutoCorrectorEngine.PlagiarismChecker.PlagiarismPair)button.DataContext;

            IEventAggregator eventAggregator = ((App)Application.Current).ServiceProvider.GetRequiredService<EventAggregator>();

            await eventAggregator.PublishAsync(new NavigationRequestEvent(typeof(PlagiarismViewModel)), marshal: async action => await action());
        }
    }
}
