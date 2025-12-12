using System.Numerics;
using Engine.Enums;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;

namespace Engine.Components;

public class SpriteComponent : Component, IDrawable
{
    public RenderSpace RenderSpace { get; set; } = RenderSpace.World;
    
    public int Width, Height;
    public Color Color;

    public void Draw(float alpha)
    {
        Graphics.DrawRectangleV(Entity.Transform.LocalRenderPosition, new Vector2(Width, Height), Color);
    }
}