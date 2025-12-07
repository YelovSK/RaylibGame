using System.Text.Json;
using System.Text.Json.Serialization;
using Engine;
using Raylib_CSharp.Interact;

namespace Game.Persistence;

[JsonSerializable(typeof(Settings))]
internal partial class SettingsJsonContext : JsonSerializerContext
{
}

public class Settings : Singleton<Settings>
{
    public KeyboardKey JumpKey { get; set; } = KeyboardKey.C;
    public KeyboardKey DashKey { get; set; } = KeyboardKey.X;
    public bool IsFullScreen { get; set; } = false;
    public bool IsVsyncEnabled { get; set; } = true;
    public bool ShowFps { get; set; } = false;
    public bool EnableShaders { get; set; } = true;
    
    private static readonly string SETTINGS_PATH = Path.Join(Constants.GAME_DATA_PATH, "settings.json");
    
    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SETTINGS_PATH)!);

        var json = JsonSerializer.Serialize(this, SettingsJsonContext.Default.Settings);
        File.WriteAllText(SETTINGS_PATH, json);
    }

    public void Load()
    {
        if (File.Exists(SETTINGS_PATH))
        {
            var json = File.ReadAllText(SETTINGS_PATH);
            Instance = JsonSerializer.Deserialize(json, SettingsJsonContext.Default.Settings);
        }
    }
}