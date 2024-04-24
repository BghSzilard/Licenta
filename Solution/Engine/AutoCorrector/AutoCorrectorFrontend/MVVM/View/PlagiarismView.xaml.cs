using System.Windows;
using System.Windows.Controls;
using AutoCorrectorFrontend.Events;
using AutoCorrectorFrontend.MVVM.ViewModel;
using Caliburn.Micro;
using Microsoft.Extensions.DependencyInjection;

namespace AutoCorrectorFrontend.MVVM.View
{
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

            await eventAggregator.PublishAsync(new NavigationRequestEvent(typeof(PlagiarismViewModel), row), marshal: async action => await action());
        }
    }
}
