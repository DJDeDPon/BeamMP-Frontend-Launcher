using System.Collections.Generic;
using ReactiveUI;

namespace BeamMPLauncher.Models;

public class ServerConfig : ReactiveObject
{
    private GeneralConfig _general = new GeneralConfig();
    public GeneralConfig General
    {
        get => _general;
        set => this.RaiseAndSetIfChanged(ref _general, value);
    }
    
    private MiscConfig _misc = new MiscConfig();
    public MiscConfig Misc
    {
        get => _misc;
        set => this.RaiseAndSetIfChanged(ref _misc, value);
    }
}

public class GeneralConfig : ReactiveObject
{
    private int _port = 30814;
    public int Port
    {
        get => _port;
        set => this.RaiseAndSetIfChanged(ref _port, value);
    }

    private string _authKey = "";
    public string AuthKey
    {
        get => _authKey;
        set => this.RaiseAndSetIfChanged(ref _authKey, value);
    }

    private bool _allowGuests = true;
    public bool AllowGuests
    {
        get => _allowGuests;
        set => this.RaiseAndSetIfChanged(ref _allowGuests, value);
    }

    private bool _logChat = true;
    public bool LogChat
    {
        get => _logChat;
        set => this.RaiseAndSetIfChanged(ref _logChat, value);
    }

    private bool _debug = false;
    public bool Debug
    {
        get => _debug;
        set => this.RaiseAndSetIfChanged(ref _debug, value);
    }

    private string _ip = "::";
    public string IP
    {
        get => _ip;
        set => this.RaiseAndSetIfChanged(ref _ip, value);
    }

    private bool _private = true;
    public bool Private
    {
        get => _private;
        set => this.RaiseAndSetIfChanged(ref _private, value);
    }

    private bool _informationPacket = true;
    public bool InformationPacket
    {
        get => _informationPacket;
        set => this.RaiseAndSetIfChanged(ref _informationPacket, value);
    }

    private string _name = "BeamMP Server";
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    private string _tags = "Freeroam";
    public string Tags
    {
        get => _tags;
        set => this.RaiseAndSetIfChanged(ref _tags, value);
    }

    private int _maxCars = 1;
    public int MaxCars
    {
        get => _maxCars;
        set => this.RaiseAndSetIfChanged(ref _maxCars, value);
    }

    private int _maxPlayers = 8;
    public int MaxPlayers
    {
        get => _maxPlayers;
        set => this.RaiseAndSetIfChanged(ref _maxPlayers, value);
    }

    private string _map = "/levels/gridmap_v2/info.json";
    public string Map
    {
        get => _map;
        set => this.RaiseAndSetIfChanged(ref _map, value);
    }

    private string _description = "BeamMP Default Description";
    public string Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }

    private string _resourceFolder = "Resources";
    public string ResourceFolder
    {
        get => _resourceFolder;
        set => this.RaiseAndSetIfChanged(ref _resourceFolder, value);
    }
}

public class MiscConfig : ReactiveObject
{
    private bool _imScaredOfUpdates = true;
    public bool ImScaredOfUpdates
    {
        get => _imScaredOfUpdates;
        set => this.RaiseAndSetIfChanged(ref _imScaredOfUpdates, value);
    }

    private string _updateReminderTime = "30s";
    public string UpdateReminderTime
    {
        get => _updateReminderTime;
        set => this.RaiseAndSetIfChanged(ref _updateReminderTime, value);
    }
}
