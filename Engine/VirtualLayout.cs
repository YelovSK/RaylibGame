using System.Numerics;
using Engine.Enums;

namespace Engine.Helpers;

public static class VirtualLayout
{
    public static Vector2 Center(float width = 0f, float height = 0f)
    {
        return new Vector2(
            (float)Application.Instance.VirtualWidth / 2 - width / 2,
            (float)Application.Instance.VirtualHeight / 2 - height / 2
        );
    }

    public static Vector2 AnchorToScreen(
        int width,
        int height,
        Anchor anchor = Anchor.TopLeft
    )
    {
        var screenWidth = (float)Application.Instance.VirtualWidth;
        var screenHeight = (float)Application.Instance.VirtualHeight;
        
        return anchor switch
        {
            Anchor.TopLeft => new Vector2(0, 0),
            Anchor.TopCenter => new Vector2((screenWidth - width) / 2f, 0),
            Anchor.TopRight => new Vector2(screenWidth - width, 0),

            Anchor.CenterLeft => new Vector2(0, (screenHeight - height) / 2f),
            Anchor.Center => new Vector2((screenWidth - width) / 2f, (screenHeight - height) / 2f),
            Anchor.CenterRight => new Vector2(screenWidth - width, (screenHeight - height) / 2f),

            Anchor.BottomLeft => new Vector2(0, screenHeight - height),
            Anchor.BottomCenter => new Vector2((screenWidth - width) / 2f, screenHeight - height),
            Anchor.BottomRight => new Vector2(screenWidth - width, screenHeight - height),

            _ => throw new ArgumentOutOfRangeException()
        };
    }
}