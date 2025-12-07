using System.Numerics;
using Engine.Components;

namespace Engine;

public class Entity
{
    public readonly TransformComponent Transform;
    public readonly Scene Scene;
    private readonly List<Component> _components = [];

    public Entity(Scene scene)
    {
        Scene = scene;
        Transform = new TransformComponent();
        AddComponent(Transform);
    }
    
    public Entity(Scene scene, Vector2 position)
    {
        Scene = scene;
        Transform = new TransformComponent()
        {
            Position = position
        };
        AddComponent(Transform);
    }

    public T AddComponent<T>(T c) where T : Component
    {
        c.Entity = this;
        _components.Add(c);
        return c;
    }

    public T? GetComponent<T>() where T : Component
    {
        return _components.OfType<T>().FirstOrDefault();
    }

    public void Start()
    {
        foreach (var component in _components)
        {
            component.Start();
        }
    }

    public void Update(float dt)
    {
        foreach (var c in _components)
        {
            c.Update(dt);
        }
    }
    
    public void FixedUpdate()
    {
        foreach (var c in _components)
        {
            c.FixedUpdate();
        }
    }

    public void Draw()
    {
        foreach (var c in _components)
        {
            c.Draw();
        }
    }
    
    public void OnDestroy()
    {
        foreach (var c in _components)
        {
            c.OnDestroy();
        }
    }
}