using System.Text.Json;
using System.Text.Json.Serialization;
using Engine;

namespace Game.Persistence;

[JsonSerializable(typeof(SaveData))]
internal partial class SaveDataJsonContext : JsonSerializerContext
{
}

public class SaveData : Singleton<SaveData>
{
    private static readonly string SAVE_PATH = Path.Join(Constants.GAME_DATA_PATH, "save.json");
    
    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SAVE_PATH)!);

        var json = JsonSerializer.Serialize(this, SaveDataJsonContext.Default.SaveData);
        File.WriteAllText(SAVE_PATH, json);
    }

    public void Load()
    {
        if (File.Exists(SAVE_PATH))
        {
            var json = File.ReadAllText(SAVE_PATH);
            
            var save = JsonSerializer.Deserialize(json, SaveDataJsonContext.Default.SaveData) ?? new SaveData();
            foreach (var property in typeof(SaveData).GetProperties().Where(p => p.CanWrite))
            {
                property.SetValue(this, property.GetValue(save, null), null);
            }
        }
    }
}