using System.Numerics;
using Engine.Components;

namespace Engine;

public class Entity
{
    public readonly TransformComponent Transform;
    public Scene Scene;
    private readonly List<Component> _components = [];
    public ReadOnlyList<Component> Components => new(_components);

    public Entity() : this(Vector2.Zero) { }

    public Entity(Vector2 position)
    {
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
