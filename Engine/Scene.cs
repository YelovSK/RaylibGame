using Engine.Components;
using Engine.Enums;
using Raylib_CSharp.Rendering;

namespace Engine;

public abstract class Scene
{
    public ReadOnlyHashSet<Entity> Entities => new(_entities);
    public ReadOnlyHashSet<CameraComponent> Cameras => new(_cameras);
    
    /// <summary>
    /// List of entities that have been marked for destroy.
    /// Have to wait until the current loop finishes.
    /// </summary>
    private readonly HashSet<Entity> _entitiesToDestroy = [];
    private readonly HashSet<Entity> _entities = [];
    private readonly HashSet<CameraComponent> _cameras = [];
    
    // Is around 50% faster when scene has a bunch of components.
    // Have to somehow keep it up to date which is annoying.
    // Plus there is a bigger overhead when adding/removing components.
    private readonly HashSet<IUpdatable> _updatables = [];
    private readonly HashSet<IDrawable> _screenDrawables = [];
    private readonly HashSet<IDrawable> _worldDrawables = [];
    
    // Camera components register themselves.
    // Right now, only a single camera is used.
    public CameraComponent? Camera => _cameras.FirstOrDefault();

    /// <summary>
    /// Initialize entities etc here.
    /// </summary>
    public abstract void Load();

    public void Start()
    {
        foreach (var entity in _entities)
        {
            foreach (var component in entity.Components)
            {
                component.Start();
            }
        }
    }

    public Entity CreateEntity()
    {
        var entity = new Entity(this);
        _entities.Add(entity);
        return entity;
    }

    public virtual void Update(float dt)
    {
        foreach (var update in _updatables)
        {
            update.Update(dt);
        }
        
        InternalDestroy();
    }

    public void Draw()
    {
        DrawWorldSpace();
        DrawScreenSpace();
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
        
        // I guess it's better to pay the price when changing a scene than during gameplay?
        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
    }

    public void Destroy(Entity entity) => _entitiesToDestroy.Add(entity);

    public void RegisterComponent<T>(T component) where T : Component
    {
        if (component is IUpdatable updateable)
        {
            _updatables.Add(updateable);
        }
        
        if (component is IDrawable drawable)
        {
            switch (drawable.RenderSpace)
            {
                case RenderSpace.Screen:
                    _screenDrawables.Add(drawable);
                    break;
                case RenderSpace.World:
                    _worldDrawables.Add(drawable);
                    break;
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
    
    private void DrawWorldSpace()
    {
        if (Camera != null)
        {
            Graphics.BeginMode2D(Camera.Camera);
        }
        
        foreach (var draw in _worldDrawables)
        {
            draw.Draw();
        }
        
        if (Camera != null)
        {
            Graphics.EndMode2D();
        }
    }

    private void DrawScreenSpace()
    {
        foreach (var draw in _screenDrawables)
        {
            draw.Draw();
        }
    }

    /// <summary>
    /// Destroys entities that were marked for destroy.
    /// </summary>
    private void InternalDestroy()
    {
        if (_entitiesToDestroy.Count == 0)
        {
            return;
        }
        
        foreach (var entity in _entitiesToDestroy)
        {
            // Remove components
            foreach (var component in entity.Components)
            {
                if (component is IDrawable drawable)
                {
                    switch (drawable.RenderSpace)
                    {
                        case RenderSpace.World:
                            _worldDrawables.Remove(drawable);
                            break;
                        case RenderSpace.Screen:
                            _screenDrawables.Remove(drawable);
                            break;
                    }
                }
            
                if (component is IUpdatable updatable)
                {
                    _updatables.Remove(updatable);
                }
            
                component.OnDestroy();
            }
        
            _entities.Remove(entity);
        }

        _entitiesToDestroy.Clear();
    }
}