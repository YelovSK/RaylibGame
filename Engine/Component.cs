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
    RenderSpace RenderSpace { get; set; }
    
    void Draw();
}

/// <summary>
/// Components that need smooth rendering between fixed timestep physics updates.
/// SavePreviousState is called before each FixedUpdate.
/// ComputeRenderState is called before Draw with interpolation alpha (0-1).
/// </summary>
public interface IInterpolatable
{
    /// <summary>
    /// Called before FixedUpdate to save the current state for interpolation.
    /// </summary>
    void SavePreviousState();
    
    /// <summary>
    /// Called before Draw to compute the interpolated render state.
    /// </summary>
    /// <param name="alpha">Interpolation factor between previous (0) and current (1) state</param>
    void ComputeRenderState(float alpha);
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