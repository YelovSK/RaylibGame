using System.Numerics;
using Engine.Extensions;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Interact;

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
        var mousePosition = Input.GetVirtualMousePosition();
        IsHovered = ShapeHelper.CheckCollisionPointRec(mousePosition, Rect.Bounds);

        if (IsHovered && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            _wasPressed = true;
        }

        var clicked = _wasPressed &&
                      Input.IsMouseButtonReleased(MouseButton.Left);
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
