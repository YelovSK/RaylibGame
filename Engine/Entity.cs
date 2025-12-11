using System.Numerics;
using Engine.Components;

namespace Engine;

public class Entity
{
    public readonly TransformComponent Transform;
    public readonly Scene Scene;
    private readonly List<Component> _components = [];
    public ReadOnlyList<Component> Components => new(_components);

    public Entity(Scene scene) : this(scene, Vector2.Zero) { }

    public Entity(Scene scene, Vector2 position)
    {
        Scene = scene;
        Transform = new TransformComponent { Position = position };
        scene.RegisterEntity(this);
        AddComponent(Transform);
    }

    public T AddComponent<T>(T c) where T : Component
    {
        c.Entity = this;
        _components.Add(c);
        Scene.RegisterComponent(c);
        return c;
    }
    
    public void RemoveComponent<T>(T c) where T : Component
    {
        Scene.UnregisterComponent(c);
        _components.Remove(c);
    }

    public T? GetComponent<T>() where T : Component
    {
        foreach (var component in _components)
        {
            if (component is T result)
            {
                return result;
            }
        }

        return null;
    }
}
