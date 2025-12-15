using Engine.Extensions;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Interact;

namespace Engine.Components;

public enum InteractableState
{
    Normal,
    Hoverered,
    Pressed,
}

public class GuiInteractableComponent : Component, IUpdatable
{
    public bool IsHovered;
    public bool WasPressed;
    public bool IsClicked;
    
    public InteractableState State { get; private set; }

    private RectTransform? _rectTransform;

    public override void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Update(float dt)
    {
        IsClicked = false;
        
        if (_rectTransform is null)
        {
            return;
        }
        
        var mousePos = Input.GetVirtualMousePosition();

        IsHovered = ShapeHelper.CheckCollisionPointRec(mousePos, _rectTransform.Rectangle);

        if (IsHovered && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            WasPressed = true;
        }

        if (WasPressed && Input.IsMouseButtonReleased(MouseButton.Left))
        {
            if (IsHovered)
            {
                IsClicked = true;
            }

            WasPressed = false;
        }
        
        if (WasPressed && IsHovered)
        {
            State = InteractableState.Pressed;
        }
        else if (IsHovered)
        {
            State = InteractableState.Hoverered;
        }
        else
        {
            State = InteractableState.Normal;
        }
    }
}