using System.Numerics;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Windowing;

namespace Engine;

public static class VirtualViewport
{
    public static int Width { get; private set; }
    public static int Height { get; private set; }

    internal static void Initialize(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public static Rectangle Canvas => new(
        0,
        0,
        Width,
        Height
    );

    public static Vector2 Center => Canvas.Size / 2;

    public static Rectangle Destination
    {
        get
        {
            var screenWidth = Window.GetScreenWidth();
            var screenHeight = Window.GetScreenHeight();
            var scale = Math.Min(
                screenWidth / (float)Width,
                screenHeight / (float)Height
            );

            return new Rectangle(
                (screenWidth - Width * scale) / 2,
                (screenHeight - Height * scale) / 2,
                Width * scale,
                Height * scale
            );
        }
    }

    public static Vector2 ScreenToVirtual(Vector2 position)
    {
        var destination = Destination;
        var scale = destination.Width / Width;
        return (position - destination.Position) / scale;
    }
}
