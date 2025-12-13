using Raylib_CSharp.Textures;

namespace Engine;

public partial class World
{
    private uint _nextEntityId = 1;
    private readonly Dictionary<Type, IComponentPool> _componentPools = new();
    private readonly List<ISystem> _systems = [];
    private readonly List<IRenderSystem> _renderSystems = [];
    
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
    
    public void AddSystem(ISystem system)
    {
        _systems.Add(system);
    }
    
    public void AddRenderSystem(IRenderSystem system)
    {
        _renderSystems.Add(system);
    }
    
    public void Update(float dt)
    {
        foreach (var system in _systems)
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