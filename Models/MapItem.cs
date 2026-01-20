using Avalonia.Media.Imaging;
using Newtonsoft.Json;

namespace BeamMPLauncher.Models;

public class MapItem
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public string Preview { get; set; } = "";
    
    [JsonIgnore]
    public Bitmap? Thumbnail { get; set; }
}
