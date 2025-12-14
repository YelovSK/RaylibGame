namespace Engine;

public interface ISystem
{
    void Update(World world, float dt);
}

/// <summary>
/// Runs at a fixed rate <see cref="FixedTime.TICK_RATE"/>
/// </summary>
public interface IPhysicsSystem
{
    void Update(World world, float dt);
}

public interface IRenderSystem
{
    void Draw(World world);
}
