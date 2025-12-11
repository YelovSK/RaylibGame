using Engine.Components;

namespace Engine;

/// <summary>
/// Create via <see cref="Scene.CreateEntity()"/>
/// </summary>
public class Entity
{
    public readonly TransformComponent Transform;
    public readonly Scene Scene;
    private readonly List<Component> _components;
    public ReadOnlyCollections<Component> Components => new(_components);

    internal Entity(Scene scene)
    {
        Scene = scene;
        Transform = new TransformComponent { Entity = this };
        _components = [Transform];
    }

    public T AddComponent<T>() where T : Component, new()
    {
        var component = new T { Entity = this };
        _components.Add(component);
        Scene.RegisterComponent(component);
        return component;
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
