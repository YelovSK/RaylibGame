using System.Numerics;
using Engine.Extensions;

using Raylib_cs;

namespace Engine.Components;

public enum InteractableState
{
    Normal,
    Hovered,
    Pressed,
}

public abstract class UiControlComponent : UiComponent, IUpdatable
{
    public bool IsHovered { get; private set; }
    public InteractableState State { get; private set; }

    private bool _wasPressed;

    public void Update(float dt)
    {
        var mousePosition = Raylib.GetVirtualMousePosition();
        IsHovered = Raylib.CheckCollisionPointRec(mousePosition, Rect.Bounds);

        if (IsHovered && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            _wasPressed = true;
        }

        var clicked = _wasPressed &&
                      Raylib.IsMouseButtonReleased(MouseButton.Left);
        if (clicked)
        {
            _wasPressed = false;
        }

        State = _wasPressed && IsHovered
            ? InteractableState.Pressed
            : IsHovered
                ? InteractableState.Hovered
                : InteractableState.Normal;

        UpdateControl(dt, mousePosition);

        if (clicked && IsHovered)
        {
            Click();
        }
    }

    protected virtual void UpdateControl(float dt, Vector2 mousePosition)
    {
    }

    protected virtual void Click()
    {
    }
}
