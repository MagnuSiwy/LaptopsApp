using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Magnuszewski.LaptopsApp.Interfaces;
using Magnuszewski.LaptopsApp.ViewModels;
using Magnuszewski.LaptopsApp;

namespace LaptopsApp
{
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            // string daoAssemblyPath = @"C:\Users\student\Documents\LaptopsApp\DAOFile\bin\Debug\net8.0-windows\DAOFile.dll";
            string daoAssemblyPath = @"C:\Users\student\Documents\LaptopsApp\DAOSQL\bin\Debug\net8.0-windows\DAOSQL.dll";
            // string daoAssemblyPath = @"C:\Users\student\Documents\LaptopsApp\DAOMock\bin\Debug\net8.0-windows\DAOMock.dll";
            ConfigureServices(serviceCollection, daoAssemblyPath);
            serviceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services, string daoAssemblyPath)
        {
            Assembly daoAssembly = Assembly.LoadFrom(daoAssemblyPath);

            Type laptopStorageType = daoAssembly.GetTypes()
                .FirstOrDefault(t => typeof(ILaptopStorage).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            if (laptopStorageType == null)
            {
                throw new InvalidOperationException("No implementation of ILaptopStorage found in the specified assembly.");
            }

            services.AddSingleton(typeof(ILaptopStorage), laptopStorageType);

            services.AddTransient<MainViewModel>(provider =>
            {
                var laptopStorage = provider.GetRequiredService<ILaptopStorage>();
                return new MainViewModel(laptopStorage);
            });
            services.AddTransient<MainWindow>(provider =>
            {
                var viewModel = provider.GetRequiredService<MainViewModel>();
                return new MainWindow(viewModel);
            });
        }
    }
}