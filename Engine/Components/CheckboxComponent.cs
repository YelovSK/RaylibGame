using System.Numerics;
using Engine.Enums;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;

namespace Engine.Components;

public class CheckboxComponent : Component, IUpdatable, IDrawable
{
    public RenderSpace RenderSpace { get; set; } = RenderSpace.Screen;
    
    public bool IsChecked { get; set; }
    public Action<bool>? OnClick;
    
    public float StrokeWidth = 2f;
    public Color NormalColor = Color.DarkGray;
    public Color HoverColor = Color.Gray;
    public Color PressedColor = Color.LightGray;
    
    private GuiInteractableComponent? _guiInteractableComponent;
    private RectTransform? _rectTransform;

    public override void Start()
    {
        _guiInteractableComponent = GetComponent<GuiInteractableComponent>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Update(float dt)
    {
        if (_rectTransform is null || _guiInteractableComponent is null)
        {
            return;
        }

        if (_guiInteractableComponent.IsClicked)
        {
            IsChecked = !IsChecked;
            OnClick?.Invoke(IsChecked);
        }
    }

    public void Draw()
    {
        if (_rectTransform is null)
        {
            return;
        }

        var pos = _rectTransform.Rectangle.Position;
        var size = _rectTransform.Rectangle.Size;

        // Choose color based on state
        var bgColor = _guiInteractableComponent?.State switch
        {
            InteractableState.Normal => NormalColor,
            InteractableState.Hoverered => HoverColor,
            InteractableState.Pressed => PressedColor,
            _ => NormalColor,
        };

        // Draw button background
        Graphics.DrawRectangleV(pos, size, bgColor);
        Graphics.DrawRectangleLinesEx(
            _rectTransform.Rectangle,
            2,
            Color.Black
        );

        if (IsChecked)
        {
            Graphics.DrawLineEx(pos, new Vector2(pos.X + size.X, pos.Y + size.Y), StrokeWidth, Color.Black);
            Graphics.DrawLineEx(pos with { X = pos.X + size.X }, pos with { Y = pos.Y + size.Y }, StrokeWidth, Color.Black);
        }
    }
}