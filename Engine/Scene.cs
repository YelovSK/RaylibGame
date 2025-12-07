namespace Engine;

public abstract class Scene
{
    public List<Entity> Entities { get; } = new();

    public virtual void Load()
    {
    }

    public virtual void Start()
    {
        foreach (var obj in Entities)
        {
            obj.Start();
        }
    }

    public void AddEntity(Entity obj) => Entities.Add(obj);

    public void RemoveEntity(Entity obj)
    {
        obj.OnDestroy();
        Entities.Remove(obj);
    }

    public virtual void Update(float dt)
    {
        foreach (var obj in Entities)
        {
            obj.Update(dt);
        }
    }
    
    public virtual void FixedUpdate()
    {
        foreach (var obj in Entities)
        {
            obj.FixedUpdate();
        }
    }

    public virtual void Draw()
    {
        foreach (var obj in Entities)
        {
            obj.Draw();
        }
    }
    
    public virtual void OnDestroy()
    {
        foreach (var obj in Entities)
        {
            obj.OnDestroy();
        }
    }
}