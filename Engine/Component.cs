using Engine.Enums;

namespace Engine;

public interface IUpdatable
{
    void Update(float dt);
}

public interface IFixedUpdatable
{
    void FixedUpdate();
}

public interface ILateUpdatable
{
    void LateUpdate(float dt);
}

public interface IDrawable
{
    /// <summary>
    /// Uses Camera2D if set to <see cref="RenderSpace.World"/> and a camera is present in a scene.
    /// </summary>
    public RenderSpace RenderSpace { get; set; }
    
    void Draw(float alpha);
}

public abstract class Component
{
    /// <summary>
    /// Should be set only in <see cref="Scene.CreateEntity()"/>
    /// </summary>
    public Entity Entity { get; internal init; } = null!;

    public virtual void Start()
    {
    }

    public virtual void OnDestroy()
    {
        
    }
    
    public T? GetComponent<T>() where T : Component => Entity.GetComponent<T>();
    
    public IEnumerable<T> GetComponents<T>() where T : Component => Entity.GetComponents<T>();
}