using System.Numerics;
using Raylib_CSharp.Colors;

namespace Engine.Components;

public struct RectTransform
{
    public Vector2 Position;
    public Vector2 Size;
}

public struct UiPointerStateComponent
{
    public bool IsHovered;
    public bool IsPressed;
    public bool IsClicked;
    /// <summary>
    /// Yes, obviously this is bad. Honestly, fuck ECS. It's so ass.
    /// </summary>
    public Action? Action;
}

public struct LabelComponent
{
    public string Text;
    public int FontSize;
    public Color TextColor;
}