namespace Engine;

public partial class World
{
    private int _nextEntityId = 1;
    private readonly Dictionary<Type, IComponentPool> _componentPools = new();
    private List<ISystem> _systems = [];
    private List<IPhysicsSystem> _physicsSystems = [];
    private List<IRenderSystem> _renderSystems = [];
    
    public Entity CreateEntity()
    {
        return new Entity(_nextEntityId++);
    }
    
    public void DestroyEntity(Entity e)
    {
        foreach (var pool in _componentPools.Values)
        {
            pool.Delete(e);
        }
    }
    
    public ref T AddComponent<T>(Entity e, in T value) where T : struct
    {
        return ref GetPool<T>().Set(e, value);
    }

    public ref T GetComponent<T>(Entity e) where T : struct 
    {
        return ref GetPool<T>().GetRef(e);
    }

    public bool HasComponent<T>(Entity e) where T : struct
    {
        return GetPool<T>().Contains(e);
    }
    
    public void AddSystem<T>() where T : ISystem, new()
    {
        _systems.Add(new T());
    }
    
    public void AddPhysicsSystem<T>() where T : IPhysicsSystem, new()
    {
        _physicsSystems.Add(new T());
    }
    
    public void AddRenderSystem<T>() where T : IRenderSystem, new()
    {
        _renderSystems.Add(new T());
    }

    public void CompileSystems()
    {
        _systems = SystemScheduler.Build(_systems);
        _physicsSystems = SystemScheduler.Build(_physicsSystems);
        _renderSystems = SystemScheduler.Build(_renderSystems);
    }
    
    public void Update(float dt)
    {
        foreach (var system in _systems)
        {
            system.Update(this, dt);
        }
    }
    
    public void PhysicsUpdate(float dt)
    {
        foreach (var system in _physicsSystems)
        {
            system.Update(this, dt);
        }
    }
    
    public void Draw()
    {
        foreach (var system in _renderSystems)
        {
            system.Draw(this);
        }
    }

    public SparseSet<T> GetPool<T>() where T : struct
    {
        if (_componentPools.TryGetValue(typeof(T), out var pool))
        {
            return (SparseSet<T>)pool;
        }
        
        pool = new SparseSet<T>();
        _componentPools.Add(typeof(T), pool);

        return (SparseSet<T>)pool;
    }
}