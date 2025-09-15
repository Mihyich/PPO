using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using MetroGidDesktop.ViewModels;
using MetroGidDesktop.Views;
using System;
using Microsoft.Extensions.DependencyInjection;
using MetroGidDesktop.Services.Interfaces;
using MetroGidDesktop.Services.Concrete;

namespace MetroGidDesktop;

public partial class App : Application
{
    public static Window? MainWindow { get; set; }
    public static IServiceProvider Services { get; private set; } = default!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var serviceCollection = new ServiceCollection();

            // Services
            serviceCollection.AddHttpClient<IAuthService, AuthService>(
                client =>
                {
                    client.BaseAddress = new Uri("http://localhost:5000/");
                }
            );
            serviceCollection.AddSingleton<IAuthState, AuthState>();
            serviceCollection.AddSingleton<IDialogService, DialogService>();
            // ViewModels
            serviceCollection.AddSingleton<MainWindowViewModel>();
            serviceCollection.AddSingleton<AuthViewModel>();
            serviceCollection.AddSingleton<RouteViewModel>();
            // Locator
            serviceCollection.AddSingleton<ViewLocator>();

            Services = serviceCollection.BuildServiceProvider();

            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>()
            };

            MainWindow = desktop.MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}