using System.Numerics;
using Engine.Enums;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;

namespace Engine.Components;

/// <summary>
/// World-space rectangle.
/// </summary>
public class SpriteComponent : Component, IDrawable
{
    public RenderSpace RenderSpace { get; set; } = RenderSpace.World;
    
    public int Width, Height;
    public Color Color;

    public void Draw()
    {
        Graphics.DrawRectangleV(Entity.Transform.RenderPosition, new Vector2(Width, Height), Color);
    }
}