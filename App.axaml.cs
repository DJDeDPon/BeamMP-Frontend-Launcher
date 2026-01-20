using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BeamMPLauncher.ViewModels;
using BeamMPLauncher.Views;
using BeamMPLauncher.Services;
using System.IO;
using System;

namespace BeamMPLauncher;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Initialize basic services
            string baseDir = AppContext.BaseDirectory;
            string configPath = Path.Combine(baseDir, "ServerConfig.toml");

            // If not found in current dir, check parent (backward compatibility/dev structure)
            if (!File.Exists(configPath))
            {
                string parentPath = Path.Combine(baseDir, "..", "ServerConfig.toml");
                if (File.Exists(parentPath))
                {
                    configPath = parentPath;
                }
            }
            
            var configService = new ConfigService(configPath);
            
            // Fix: Pass full path of maps.json to MapService
            string mapsPath = Path.Combine(baseDir, "maps.json");
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(configService, mapsPath),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
