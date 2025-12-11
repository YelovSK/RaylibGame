namespace Engine;

public class SceneManager : Singleton<SceneManager>
{
    private const float FIXED_DELTA_TIME = 1f / 60f;
    private float _accumulator = 0f;

    public Scene? Current
    {
        get
        {
            _scenes.TryPeek(out var scene);
            return scene;
        }
    }

    private readonly Stack<Scene> _scenes = [];
    
    /// <summary>
    /// Removes all scenes from stack and adds a scene.
    /// </summary>
    public void Load(Scene scene)
    {
        _scenes.Clear();
        Push(scene);
    }

    public bool HasScene<T>() where T : Scene
    {
        foreach (var scene in _scenes)
        {
            if (scene is T)
            {
                return true;
            }
        }

        return false;
    }

    public void Push(Scene scene)
    {
        _scenes.Push(scene);
        scene.Load();
        scene.Start();
    }
    
    public void Pop()
    {
        var success = _scenes.TryPop(out var scene);
        if (success)
        {
            scene!.OnDestroy();
        }
    }

    public void Update(float dt)
    {
        Current?.Update(dt);
        
        _accumulator += dt;
        if (_accumulator >= FIXED_DELTA_TIME) {
            Current?.FixedUpdate();
            _accumulator = 0f;
        }
    }

    public void Draw()
    {
        Current?.Draw();
    }
}