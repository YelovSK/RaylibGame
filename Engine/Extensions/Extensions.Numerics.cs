using System.Numerics;
using Raylib_CSharp.Transformations;

namespace Engine.Extensions;

public static class ExtensionsNumerics
{
    extension(Rectangle rect)
    {
        public Vector2 TopLeft() => rect.Position;
        public Vector2 TopRight() => new(rect.X + rect.Width, rect.Y);
        public Vector2 BottomLeft() => new(rect.X, rect.Y + rect.Height);
        public Vector2 BottomRight() => new(rect.X + rect.Width, rect.Y + rect.Height);
        public Vector2 Center() => new(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
    }

    extension(Vector2 value)
    {
        public static Vector2 X(float x) => new Vector2(x, 0);
        public static Vector2 Y(float y) => new Vector2(0, y);
    }
}