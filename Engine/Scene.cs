namespace Engine;

/// <summary>
/// Basically just a container for a world.
/// </summary>
public abstract class Scene
{
    protected World World { get; private set; } = null!;

    internal void InternalLoad()
    {
        World = new World();
        Load(World);
        World.CompileSystems();
    }

    /// <summary>
    /// Initialize entities and components here.
    /// </summary>
    protected abstract void Load(World world);

    public void Update(float dt) => World.Update(dt);
    
    public void FixedUpdate(float dt) => World.PhysicsUpdate(dt);

    public void Draw() => World.Draw();
}