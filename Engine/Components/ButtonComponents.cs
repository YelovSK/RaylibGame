using Raylib_CSharp.Colors;

namespace Engine.Components;

public struct ButtonStyleComponent
{
    public float StrokeWidth;

    public Color NormalColor;
    public Color HoverColor;
    public Color PressedColor;
}

public struct ButtonVisualStateComponent
{
    public float TiltX;
    public float TiltY;
}