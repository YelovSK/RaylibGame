using System.Numerics;
using Raylib_CSharp.Transformations;

namespace Engine.Extensions;

public static class ExtensionsNumerics
{
    public static Vector2 TopLeft(this Rectangle rect) => rect.Position;

    public static Vector2 TopRight(this Rectangle rect) => new(rect.X + rect.Width, rect.Y);

    public static Vector2 BottomLeft(this Rectangle rect) => new(rect.X, rect.Y + rect.Height);

    public static Vector2 BottomRight(this Rectangle rect) => new(rect.X + rect.Width, rect.Y + rect.Height);

    public static Vector2 Center(this Rectangle rect) => new(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
}