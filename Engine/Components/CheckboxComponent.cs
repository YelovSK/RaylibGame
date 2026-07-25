using System.Numerics;

using Raylib_cs;

namespace Engine.Components;

public class CheckboxComponent : UiControlComponent
{
    public bool IsChecked { get; set; }
    public Action<bool>? OnClick;

    public float StrokeWidth = 2f;
    public Color NormalColor = Color.DarkGray;
    public Color HoverColor = Color.Gray;
    public Color PressedColor = Color.LightGray;

    protected override void Click()
    {
        IsChecked = !IsChecked;
        OnClick?.Invoke(IsChecked);
    }

    public override void Draw()
    {
        var bounds = Rect.Bounds;
        var position = bounds.Position;
        var size = bounds.Size;
        var backgroundColor = State switch
        {
            InteractableState.Normal => NormalColor,
            InteractableState.Hovered => HoverColor,
            InteractableState.Pressed => PressedColor,
            _ => NormalColor,
        };

        Raylib.DrawRectangleV(position, size, backgroundColor);
        Raylib.DrawRectangleLinesEx(bounds, 2, Color.Black);

        if (IsChecked)
        {
            Raylib.DrawLineEx(position, position + size, StrokeWidth, Color.Black);
            Raylib.DrawLineEx(
                position with { X = position.X + size.X },
                position with { Y = position.Y + size.Y },
                StrokeWidth,
                Color.Black
            );
        }
    }
}
