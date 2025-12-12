namespace Engine;

public class SceneManager : Singleton<SceneManager>
{
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
        while (_scenes.Count > 0)
        {
            _scenes.Pop().OnDestroy();
        }
        
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
    
    public void FixedUpdate()
    {
        Current?.FixedUpdate();
    }

    public void Update(float dt)
    {
        Current?.Update(dt);
    }
    
    public void LateUpdate(float dt)
    {
        Current?.LateUpdate(dt);
    }

    public void Draw(float alpha)
    {
        Current?.Draw(alpha);
    }
}