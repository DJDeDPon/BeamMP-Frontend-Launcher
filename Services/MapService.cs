using System;
using System.Collections.Generic;
using System.IO;
using BeamMPLauncher.Models;
using Newtonsoft.Json;
using Avalonia.Media.Imaging;

namespace BeamMPLauncher.Services;

public class MapService
{
    private readonly string _mapsPath;

    public MapService(string mapsPath)
    {
        _mapsPath = mapsPath;
    }

    public List<MapItem> LoadMaps()
    {
        if (!File.Exists(_mapsPath))
        {
            return new List<MapItem>();
        }

        try
        {
            string jsonContent = File.ReadAllText(_mapsPath);
            var maps = JsonConvert.DeserializeObject<List<MapItem>>(jsonContent) ?? new List<MapItem>();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory; // Use BaseDirectory for reliability
            
            // Fixup paths to be absolute
            foreach (var map in maps)
            {
               // Helper to find the asset whether we are running in bin/... or elsewhere
               if (!Path.IsPathRooted(map.Preview))
               {
                   // Try to find it relative to current directory first, then AppContext
                   string localPath = Path.Combine(Directory.GetCurrentDirectory(), map.Preview);
                   if (File.Exists(localPath)) 
                   {
                       map.Preview = localPath;
                   }
                   else
                   {
                       // Fallback to base directory
                       map.Preview = Path.Combine(baseDir, map.Preview);
                   }
               }
               
               // Load Bitmap
               try
               {
                   if (File.Exists(map.Preview))
                   {
                       map.Thumbnail = new Bitmap(map.Preview);
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"Failed to load image for {map.Name}: {ex.Message}");
               }
            }
            return maps;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading maps: {ex.Message}");
            return new List<MapItem>();
        }
    }
}
