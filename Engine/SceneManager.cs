namespace Engine;

public sealed class SceneManager : Singleton<SceneManager>
{
    public Scene? Current
    {
        get
        {
            _scenes.TryPeek(out var world);
            return world;
        }
    }

    private readonly Stack<Scene> _scenes = [];

    public void Push(Scene scene)
    {
        scene.InternalLoad();
        _scenes.Push(scene);
    }
    
    public void Pop()
    {
        _ = _scenes.TryPop(out var scene);
    }

    public void Clear() => _scenes.Clear();

    public void Load(Scene scene)
    {
        Clear();
        Push(scene);
    }

    public void Update(float dt) => Current?.Update(dt);
    
    public void FixedUpdate(float dt) => Current?.FixedUpdate(dt);

    public void Draw() => Current?.Draw();
}