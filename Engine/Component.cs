namespace Engine;

public abstract class Component
{
    public Entity Entity { get; set; }

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