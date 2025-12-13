using System.Numerics;
using Engine.Enums;
using Raylib_CSharp.Colors;

namespace Engine.Components;

public struct ButtonComponent
{
    public RenderSpace RenderSpace;

    public string Text;
    public int FontSize;
    public Vector2 Size;

    public float StrokeWidth;

    public Color NormalColor;
    public Color HoverColor;
    public Color PressedColor;
    public Color TextColor;
}