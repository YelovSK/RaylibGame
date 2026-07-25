using Raylib_CSharp.Transformations;

using System.Numerics;

namespace Engine.Components;

public class RectTransform : Component
{
    public Vector2 Anchor;
    public Vector2 Pivot;
    public Vector2 Offset;
    public Vector2 Size;

    public Rectangle Bounds
    {
        get
        {
            var canvas = VirtualViewport.Canvas;
            var anchorPosition = canvas.Position + canvas.Size * Anchor;
            var position = anchorPosition + Offset - Size * Pivot;
            return new Rectangle(position.X, position.Y, Size.X, Size.Y);
        }
    }
}
