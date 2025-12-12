using Engine.Collections;
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
    
    // Micro performance optimization when many entities in a scene but they use only some of these.
    // Have to somehow keep it up to date which is annoying.
    // Plus there is a bigger overhead when adding/removing components + memory usage.
    private readonly HashSet<IUpdatable> _updatables = [];
    private readonly HashSet<IFixedUpdatable> _fixedUpdatables = [];
    private readonly HashSet<ILateUpdatable> _lateUpdatables = [];
    private readonly HashSet<IDrawable> _drawables = [];
    
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
    
    public void FixedUpdate()
    {
        foreach (var update in _fixedUpdatables)
        {
            update.FixedUpdate();
        }
    }
    
    public void Update(float dt)
    {
        foreach (var update in _updatables)
        {
            update.Update(dt);
        }
    }
    
    public void LateUpdate(float dt)
    {
        foreach (var update in _lateUpdatables)
        {
            update.LateUpdate(dt);
        }
        
        InternalDestroy();
    }

    public void Draw(float alpha)
    {
        DrawWorldSpace(alpha);
        DrawScreenSpace(alpha);
    }
    
    public void OnDestroy()
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
        if (component is IUpdatable updatable)
        {
            _updatables.Add(updatable);
        }
        
        if (component is IFixedUpdatable fixedUpdatable)
        {
            _fixedUpdatables.Add(fixedUpdatable);
        }
        
        if (component is ILateUpdatable lateUpdatable)
        {
            _lateUpdatables.Add(lateUpdatable);
        }
        
        if (component is IDrawable drawable)
        {
            _drawables.Add(drawable);
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
    
    private void DrawWorldSpace(float alpha)
    {
        if (Camera != null)
        {
            Graphics.BeginMode2D(Camera.Camera);
        }
        
        foreach (var draw in _drawables)
        {
            if (draw.RenderSpace == RenderSpace.World)
            {
                draw.Draw(alpha);
            }
        }
        
        if (Camera != null)
        {
            Graphics.EndMode2D();
        }
    }

    private void DrawScreenSpace(float alpha)
    {
        foreach (var draw in _drawables)
        {
            if (draw.RenderSpace == RenderSpace.Screen)
            {
                draw.Draw(alpha);
            }
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
                    _drawables.Remove(drawable);
                }
            
                if (component is IUpdatable updatable)
                {
                    _updatables.Remove(updatable);
                }
                
                if (component is IFixedUpdatable fixedUpdatable)
                {
                    _fixedUpdatables.Remove(fixedUpdatable);
                }
                
                if (component is ILateUpdatable lateUpdatable)
                {
                    _lateUpdatables.Remove(lateUpdatable);
                }
            
                component.OnDestroy();
            }
        
            _entities.Remove(entity);
        }

        _entitiesToDestroy.Clear();
    }
}