using DoorControlDemo.Data;
using DoorControlDemo.ViewModels;
using DoorControlDemo.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DoorControlDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider _serviceProvider;
        [STAThread]
        public static void Main()
        {
            // SDK Documentation insists on initializing the SDK at the start of the progra's' run
            CHCNetSDK.NET_DVR_Init();
            if (CHCNetSDK.NET_DVR_Init() == false)
            {
                MessageBox.Show("NET_DVR_Init Error!");
                return;
            }
            else
            {
                MessageBox.Show("NET_DVR_Init Succes!");
            }

            // Create and configure the service collection
            var services = new ServiceCollection();
            services.AddDbContext<DoorControlDbContext>(options =>
                options.UseInMemoryDatabase("DoorControlDb"));

            // Register other services and view models as needed
            services.AddSingleton<MainViewModel>();
            services.AddTransient<CreateBadgeViewModel>();
            services.AddTransient<CreateDeviceViewModel>();
            services.AddTransient<CreateUserViewModel>();
            // Add other ViewModel registrations

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Run the application
            var app = new App(serviceProvider);
            var mainWindow = new MainWindow();
            app.Run(mainWindow);
        }

        public App(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

    }
}
