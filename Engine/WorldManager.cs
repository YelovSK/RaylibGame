using Raylib_CSharp.Textures;

namespace Engine;

public class WorldManager : Singleton<WorldManager>
{
    public World? Current
    {
        get
        {
            _worlds.TryPeek(out var world);
            return world;
        }
    }

    private readonly Stack<World> _worlds = [];
    
    /// <summary>
    /// Removes all scenes from stack and adds a scene.
    /// </summary>
    public void Load(World scene)
    {
        while (_worlds.Count > 0)
        {
            _worlds.Pop();
        }
        
        Push(scene);
    }

    public void Push(World world)
    {
        _worlds.Push(world);
    }
    
    public void Pop()
    {
        _ = _worlds.TryPop(out var scene);
    }

    public void Update(float dt)
    {
        Current?.Update(dt);
    }

    public void Draw()
    {
        Current?.Draw();
    }
}