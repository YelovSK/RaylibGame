using System.Reflection;

using Raylib_cs;

namespace Engine;

public class ResourceManager : Singleton<ResourceManager>
{
    private readonly Dictionary<string, Texture2D> _textures = [];
    private readonly Dictionary<string, Sound> _sounds = [];
    private readonly Dictionary<string, Shader> _shaders = [];
    private readonly Dictionary<string, Music> _music = [];

    public static string LoadEmbedded(string name)
    {
        foreach (var assembly in new[] { Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly() }.Distinct())
        {
            var resourceName = assembly
                .GetManifestResourceNames()
                .SingleOrDefault(x => x.EndsWith(name));

            if (resourceName == null) continue;

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream!);
            return reader.ReadToEnd();
        }

        throw new Exception($"Could not find embedded shader resource: {name}");
    }

    public Texture2D LoadTexture(string path)
    {
        if (_textures.TryGetValue(path, out var tex))
        {
            return tex;
        }

        var loaded = Raylib.LoadTexture(path);
        _textures[path] = loaded;
        return loaded;
    }

    public void UnloadTexture(string path)
    {
        if (_textures.TryGetValue(path, out var tex))
        {
            Raylib.UnloadTexture(tex);
            _textures.Remove(path);
        }
    }

    /// <param name="vsName">Pass null to use a default vertex shader.</param>
    public Shader LoadShader(string? vsName, string fsName)
    {
        var key = $"{vsName ?? "0"}|{fsName}";
        if (_shaders.TryGetValue(key, out var shader))
        {
            return shader;
        }

        var vs = vsName is null
            ? null
            : LoadEmbedded(vsName);
        var fs = LoadEmbedded(fsName);
        shader = Raylib.LoadShaderFromMemory(vs, fs);
        _shaders[key] = shader;
        return shader;
    }

    public void UnloadShader(string? vsName, string fsName)
    {
        var key = $"{vsName ?? "0"}|{fsName}";
        if (_shaders.TryGetValue(key, out var shader))
        {
            Raylib.UnloadShader(shader);
            _shaders.Remove(key);
        }
    }

    public Sound LoadSound(string path)
    {
        if (_sounds.TryGetValue(path, out var sound))
        {
            return sound;
        }

        var loaded = Raylib.LoadSound(path);
        _sounds[path] = loaded;
        return loaded;
    }

    public void UnloadSound(string path)
    {
        if (_sounds.TryGetValue(path, out var sound))
        {
            Raylib.UnloadSound(sound);
            _sounds.Remove(path);
        }
    }

    public Music LoadMusic(string path)
    {
        if (_music.TryGetValue(path, out var music))
        {
            return music;
        }

        var loaded = Raylib.LoadMusicStream(path);
        _music[path] = loaded;
        return loaded;
    }

    public void UnloadMusic(string path)
    {
        if (_music.TryGetValue(path, out var music))
        {
            Raylib.UnloadMusicStream(music);
            _music.Remove(path);
        }
    }

    public void Cleanup()
    {
        foreach (var x in _textures.Values) Raylib.UnloadTexture(x);
        foreach (var x in _shaders.Values) Raylib.UnloadShader(x);
        foreach (var x in _sounds.Values) Raylib.UnloadSound(x);
        foreach (var x in _music.Values) Raylib.UnloadMusicStream(x);

        _textures.Clear();
        _shaders.Clear();
        _sounds.Clear();
        _music.Clear();
    }
}
