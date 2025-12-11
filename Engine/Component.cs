using Engine.Enums;

namespace Engine;

public interface IUpdatable
{
    void Update(float dt);
}

public interface IDrawable
{
    /// <summary>
    /// Uses Camera2D if set to <see cref="RenderSpace.World"/> and a camera is present in a scene.
    /// Has to be set PRIOR to adding the component to an entity.
    /// </summary>
    public RenderSpace RenderSpace { get; set; }
    
    void Draw();
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
}