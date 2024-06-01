using System.Windows;
using AutoCorrector;
using Caliburn.Micro;
using Microsoft.Extensions.DependencyInjection;

namespace AutoCorrectorFrontend
{
    public partial class App : Application
    {
        public ServiceProvider ServiceProvider { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<EventAggregator>();
            services.AddSingleton<MainWindow>();
        }
    }
}