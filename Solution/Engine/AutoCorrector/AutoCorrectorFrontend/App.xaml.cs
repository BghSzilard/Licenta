﻿using System.Configuration;
using System.Data;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Extensions.DependencyInjection;

namespace AutoCorrectorFrontend
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
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