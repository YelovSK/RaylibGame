using System.Numerics;

namespace Engine.Components;

public class TransformComponent : Component
{
    public Vector2 Position;
    public float Rotation;
    public Vector2 Scale = new(1, 1);
}