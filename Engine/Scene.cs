using Engine.Components;
using Engine.Enums;
using Raylib_CSharp.Rendering;

namespace Engine;

public abstract class Scene
{
    public ReadOnlyList<Entity> Entities => new(_entities);
    public ReadOnlyList<CameraComponent> Cameras => new(_cameras);
    
    private readonly List<Entity> _entities = [];
    private readonly List<CameraComponent> _cameras = [];
    
    // Camera components register themselves.
    public CameraComponent? Camera => _cameras.FirstOrDefault();

    public virtual void Load()
    {
    }

    public virtual void Start()
    {
        foreach (var entity in _entities)
        {
            foreach (var component in entity.Components)
            {
                component.Start();
            }
        }
    }

    public void AddEntity(Entity obj)
    {
        obj.Scene = this;
        _entities.Add(obj);
    }

    public void RemoveEntity(Entity obj)
    {
        foreach (var component in obj.Components)
        {
            component.OnDestroy();
        }

        _entities.Remove(obj);
    }

    public virtual void Update(float dt)
    {
        foreach (var entity in _entities)
        {
            foreach (var component in entity.Components)
            {
                component.Update(dt);
            }
        }
    }
    
    public virtual void FixedUpdate()
    {
        foreach (var entity in _entities)
        {
            foreach (var component in entity.Components)
            {
                component.FixedUpdate();
            }
        }
    }

    public virtual void Draw()
    {
        if (Camera != null)
        {
            Graphics.BeginMode2D(Camera.Camera);
        }
        
        // World space
        foreach (var entity in _entities)
        {
            foreach (var component in entity.Components)
            {
                if (component.RenderSpace == RenderSpace.World)
                {
                    component.Draw();
                }
            }
        }
        
        if (Camera != null)
        {
            Graphics.EndMode2D();
        }
        
        // Screen space
        foreach (var entity in _entities)
        {
            foreach (var component in entity.Components)
            {
                if (component.RenderSpace == RenderSpace.Screen)
                {
                    component.Draw();
                }
            }
        }
    }
    
    public virtual void OnDestroy()
    {
        foreach (var entity in _entities)
        {
            foreach (var component in entity.Components)
            {
                component.OnDestroy();
            }
        }
    }

    public void RegisterCamera(CameraComponent camera)
    {
        _cameras.Add(camera);
    }
    
    public void UnregisterCamera(CameraComponent camera)
    {
        _cameras.Remove(camera);
    }
}