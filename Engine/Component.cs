using Engine.Components;
using Engine.Enums;

namespace Engine;

public abstract class Component
{
    public Entity Entity { get; set; }
    public CameraComponent? Camera => Entity.Scene.Camera;
    
    /// <summary>
    /// Uses Camera2D if set to <see cref="RenderSpace.World"/> and a camera is present in a scene.
    /// </summary>
    public RenderSpace RenderSpace = RenderSpace.World;

    public virtual void Start()
    {
    }

    public virtual void Update(float dt)
    {
    }
    
    public virtual void FixedUpdate()
    {
    }

    public virtual void Draw()
    {
    }

    public virtual void OnDestroy()
    {
        
    }
}