using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;

namespace Engine.Components;

public class BoxColliderComponent : Component, IUpdatable
{
    public Rectangle Bounds;

    public void Update(float dt)
    {
        // Keep bounds in sync with position
        Bounds.X = Entity.Transform.Position.X;
        Bounds.Y = Entity.Transform.Position.Y;
    }

    #if DEBUG
    public void Draw()
    {
        return;
        var color = Color.Red;
        
        Graphics.DrawRectangleLines((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height, color);
    }
    #endif
}