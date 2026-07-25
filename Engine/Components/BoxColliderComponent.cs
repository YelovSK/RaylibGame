using System.Numerics;

using Raylib_cs;

namespace Engine.Components;

public class BoxColliderComponent : Component
{
    public Vector2 Offset;
    public Vector2 Size;

    public Rectangle Bounds => new(
        Entity.Transform.Position.X + Offset.X,
        Entity.Transform.Position.Y + Offset.Y,
        Size.X,
        Size.Y
    );
}
