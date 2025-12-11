using Engine.Components;
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
    public Entity Entity { get; set; }
    public CameraComponent? Camera => Entity.Scene.Camera;

    public virtual void Start()
    {
    }

    public virtual void OnDestroy()
    {
        
    }
}