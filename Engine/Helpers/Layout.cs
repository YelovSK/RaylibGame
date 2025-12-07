using System.Numerics;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Windowing;

namespace Engine.Helpers;

// TODO: remove
public static class Layout
{
    public const int VIRTUAL_WIDTH = 640;
    public const int VIRTUAL_HEIGHT = 360;

    public static Vector2 Center(float width, float height)
    {
        return new Vector2(
            (float)VIRTUAL_WIDTH / 2 - width / 2,
            (float)VIRTUAL_HEIGHT / 2 -height / 2
        );
    }
    
    public static float WidthByPercentage(float percentage)
    {
        return percentage * VIRTUAL_WIDTH;
    }

    public static float HeightByPercentage(float percentage)
    {
        return percentage * VIRTUAL_HEIGHT;
    }

    public static Vector2 Position(
        int x,
        int y,
        float width,
        float height,
        Anchor anchor = Anchor.TopLeft
    )
    {
        return anchor switch
        {
            Anchor.TopLeft => new Vector2(x, y),
            Anchor.TopCenter => new Vector2(x - width / 2, y),
            Anchor.TopRight => new Vector2(x - width, y),
            Anchor.CenterLeft => new Vector2(x, y - height / 2),
            Anchor.Center => new Vector2(x - width / 2, y - height / 2),
            Anchor.CenterRight => new Vector2(x - width, y - height / 2),
            Anchor.BottomLeft => new Vector2(x, y - height),
            Anchor.BottomCenter => new Vector2(x - width / 2, y - height),
            Anchor.BottomRight => new Vector2(x - width, y - height),
            _ => new Vector2(x, y)
        };
    }
    
    public static Vector2 GetMousePosition()
    {
        var mouse = Input.GetMousePosition();
        return mouse;
        var scale = Math.Min(
            Window.GetScreenWidth() / (float)VIRTUAL_WIDTH,
            Window.GetScreenHeight() / (float)VIRTUAL_HEIGHT
        );
    
        var offsetX = (Window.GetScreenWidth() - VIRTUAL_WIDTH * scale) / 2;
        var offsetY = (Window.GetScreenHeight() - VIRTUAL_HEIGHT * scale) / 2;
    
        return new Vector2(
            (mouse.X - offsetX) / scale,
            (mouse.Y - offsetY) / scale
        );
    }
}

public enum Anchor
{
    TopLeft,
    TopCenter,
    TopRight,
    CenterLeft,
    Center,
    CenterRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}