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
    
    // Is around 50% faster when scene has a bunch of components.
    // Have to somehow keep it up to date which is annoying.
    // Plus there is a bigger overhead when adding/removing components.
    private readonly List<IUpdatable> _updatables = [];
    private readonly List<IDrawable> _screenDrawables = [];
    private readonly List<IDrawable> _worldDrawables = [];
    
    // Camera components register themselves.
    // Right now, only a single camera is used.
    public CameraComponent? Camera => _cameras.FirstOrDefault();

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

    public void RegisterEntity(Entity entity)
    {
        _entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        foreach (var component in entity.Components)
        {
            UnregisterComponent(component);
            component.OnDestroy();
        }

        _entities.Remove(entity);
    }

    public virtual void Update(float dt)
    {
        foreach (var update in _updatables)
        {
            update.Update(dt);
        }
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
    }

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
    
    public void UnregisterComponent<T>(T component) where T : Component
    {
        if (component is IUpdatable updateable)
        {
            _updatables.Remove(updateable);
        }
        
        if (component is IDrawable drawable)
        {
            switch (drawable.RenderSpace)
            {
                case RenderSpace.Screen:
                    _screenDrawables.Remove(drawable);
                    break;
                case RenderSpace.World:
                    _worldDrawables.Remove(drawable);
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
}