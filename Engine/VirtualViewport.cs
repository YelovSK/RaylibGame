using System.Numerics;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Windowing;

namespace Engine;

public static class VirtualViewport
{
    public static Rectangle Canvas => new(
        0,
        0,
        Application.Instance.VirtualWidth,
        Application.Instance.VirtualHeight
    );

    public static Vector2 Center => Canvas.Size / 2;

    public static Rectangle Destination
    {
        get
        {
            var screenWidth = Window.GetScreenWidth();
            var screenHeight = Window.GetScreenHeight();
            var scale = Math.Min(
                screenWidth / (float)Application.Instance.VirtualWidth,
                screenHeight / (float)Application.Instance.VirtualHeight
            );

            return new Rectangle(
                (screenWidth - Application.Instance.VirtualWidth * scale) / 2,
                (screenHeight - Application.Instance.VirtualHeight * scale) / 2,
                Application.Instance.VirtualWidth * scale,
                Application.Instance.VirtualHeight * scale
            );
        }
    }

    public static Vector2 ScreenToVirtual(Vector2 position)
    {
        var destination = Destination;
        var scale = destination.Width / Application.Instance.VirtualWidth;
        return (position - destination.Position) / scale;
    }
}
