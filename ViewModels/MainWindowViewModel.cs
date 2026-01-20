using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using BeamMPLauncher.Models;
using BeamMPLauncher.Services;
using ReactiveUI;

namespace BeamMPLauncher.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ConfigService _configService;
    private readonly MapService _mapService;
    private ServerConfig _config;
    private MapItem? _selectedMap;
    private string _statusMessage = "Ready";
    private bool _isSettingsOpen;

    public MainWindowViewModel(ConfigService configService, string mapsPath)
    {
        _configService = configService;
        _mapService = new MapService(mapsPath);
        
        // Load initial data
        _config = _configService.LoadConfig();
        var maps = _mapService.LoadMaps();
        Maps = new ObservableCollection<MapItem>(maps);

        // Find current map selection
        SelectedMap = Maps.FirstOrDefault(m => m.Path == _config.General.Map) ?? Maps.FirstOrDefault();

        // Initialize Commands
        LaunchCommand = ReactiveCommand.Create(LaunchServer);
        SaveCommand = ReactiveCommand.Create(SaveConfig);
        ToggleSettingsCommand = ReactiveCommand.Create(() => IsSettingsOpen = !IsSettingsOpen);
    }

    // Properties
    public ServerConfig Config
    {
        get => _config;
        set => this.RaiseAndSetIfChanged(ref _config, value);
    }

    public ObservableCollection<MapItem> Maps { get; }

    public MapItem? SelectedMap
    {
        get => _selectedMap;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedMap, value);
            if (value != null)
            {
                Config.General.Map = value.Path;
                this.RaisePropertyChanged(nameof(Config)); // Notify config change
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    public bool IsSettingsOpen
    {
        get => _isSettingsOpen;
        set => this.RaiseAndSetIfChanged(ref _isSettingsOpen, value);
    }

    // Proxy properties for simple binding if needed
    public string ServerName
    {
        get => Config.General.Name;
        set
        {
            Config.General.Name = value;
            this.RaisePropertyChanged(nameof(ServerName));
        }
    }
    
    // Commands
    public ReactiveCommand<Unit, Unit> LaunchCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, bool> ToggleSettingsCommand { get; }

    // Logic
    private void SaveConfig()
    {
        _configService.SaveConfig(Config);
        StatusMessage = "Configuration Saved.";
    }

    private bool _isServerRunning;
    public bool IsServerRunning
    {
        get => _isServerRunning;
        set
        {
            this.RaiseAndSetIfChanged(ref _isServerRunning, value);
            this.RaisePropertyChanged(nameof(LaunchButtonText));
            this.RaisePropertyChanged(nameof(LaunchButtonColor));
        }
    }

    public string LaunchButtonText => IsServerRunning ? "STOP SERVER" : "LAUNCH SERVER";
    public string LaunchButtonColor => IsServerRunning ? "#CC0000" : "#FF6600";
    public string LaunchButtonIcon => IsServerRunning ? "M6,6H18V18H6V6Z" : "M8,5.14V19.14L19,12.14L8,5.14Z";

    private Process? _serverProcess;

    private async void LaunchServer()
    {
        if (IsServerRunning)
        {
            StopServer();
            return;
        }

        try
        {
            SaveConfig();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Config Save Failed: {ex.Message}";
            return;
        }

        StatusMessage = $"Starting Server '{Config.General.Name}'...";

        try
        {
            string baseDir = AppContext.BaseDirectory;
            // Look for server in current directory (where launcher exe is)
            string serverExePath = Path.Combine(baseDir, "BeamMP-Server.exe");
            
            // Fallback for dev environment
            if (!File.Exists(serverExePath))
            {
                serverExePath = Path.Combine(baseDir, "..", "BeamMP-Server.exe");
            }
            
            if (!File.Exists(serverExePath))
            { 
                 // Last resort, just try filename
                 serverExePath = "BeamMP-Server.exe";
            }

            if (File.Exists(serverExePath))
            {
                ProcessStartInfo startInfo;

                if (OperatingSystem.IsLinux() && serverExePath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    startInfo = new ProcessStartInfo
                    {
                        FileName = "wine",
                        Arguments = $"\"{serverExePath}\"",
                        WorkingDirectory = Path.GetDirectoryName(serverExePath),
                        UseShellExecute = false,
                        CreateNoWindow = false
                    };
                }
                else
                {
                    startInfo = new ProcessStartInfo
                    {
                        FileName = serverExePath,
                        WorkingDirectory = Path.GetDirectoryName(serverExePath),
                        UseShellExecute = true // Use shell execute to open its own window if desired, or false to embed/hide
                    };
                }

                _serverProcess = Process.Start(startInfo);
                if (_serverProcess != null)
                {
                    IsServerRunning = true;
                    StatusMessage = "Server Running...";
                    
                    try 
                    {
                        await _serverProcess.WaitForExitAsync();
                    } 
                    catch { /* Process might be killed manually */ }
                    finally
                    {
                        IsServerRunning = false;
                        StatusMessage = "Server Stopped.";
                        _serverProcess = null;
                    }
                }
                else
                {
                    StatusMessage = "Failed to start process (null).";
                }
            }
            else
            {
                StatusMessage = "Error: BeamMP-Server.exe not found.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Launch Failed: {ex.Message}";
            IsServerRunning = false;
        }
    }

    private void StopServer()
    {
        if (_serverProcess != null && !_serverProcess.HasExited)
        {
            try
            {
                StatusMessage = "Stopping Server...";
                _serverProcess.Kill(); // Force kill since we don't have a graceful stdin way easily in this setup
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error stopping: {ex.Message}";
            }
        }
    }
}
