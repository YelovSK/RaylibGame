using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;

namespace Engine.Components;

public class SpriteComponent : Component
{
    public int Width, Height;
    public Color Color;

    public override void Draw()
    {
        Graphics.DrawRectangle((int)Entity.Transform.Position.X, (int)Entity.Transform.Position.Y, Width, Height,
            Color);
    }
}