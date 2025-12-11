using System.Numerics;
using Engine.Enums;
using Engine.Extensions;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;

namespace Engine.Components;

public class CheckboxComponent : Component, IUpdatable, IDrawable
{
    public RenderSpace RenderSpace { get; set; } = RenderSpace.Screen;
    
    public bool IsChecked { get; set; }
    public Action<bool>? OnClick;
    
    public float Size;
    public float StrokeWidth = 2f;
    public Color NormalColor = Color.DarkGray;
    public Color HoverColor = Color.Gray;
    public Color PressedColor = Color.LightGray;
    
    private bool _isHovered;
    private bool _isPressed;
    private bool _wasPressed;

    public  void Update(float dt)
    {
        var mousePos = Input.GetVirtualMousePosition();
        var bounds = new Rectangle(
            Entity.Transform.Position.X,
            Entity.Transform.Position.Y,
            Size,
            Size
        );

        _isHovered = ShapeHelper.CheckCollisionPointRec(mousePos, bounds);

        if (_isHovered && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            _wasPressed = true;
        }

        if (_wasPressed && Input.IsMouseButtonReleased(MouseButton.Left))
        {
            if (_isHovered)
            {
                IsChecked = !IsChecked;
                OnClick?.Invoke(IsChecked);
            }

            _wasPressed = false;
        }
    }

    public  void Draw()
    {
        var pos = Entity.Transform.Position;

        // Choose color based on state
        var bgColor = NormalColor;
        if (_wasPressed && _isHovered)
        {
            bgColor = PressedColor;
        }
        else if (_isHovered)
        {
            bgColor = HoverColor;
        }

        // Draw button background
        Graphics.DrawRectangleV(pos, new Vector2(Size, Size), bgColor);
        Graphics.DrawRectangleLinesEx(
            new Rectangle(pos.X, pos.Y, Size, Size),
            2,
            Color.Black
        );

        if (IsChecked)
        {
            Graphics.DrawLineEx(pos, new Vector2(pos.X + Size, pos.Y + Size), StrokeWidth, Color.Black);
            Graphics.DrawLineEx(pos with { X = pos.X + Size }, pos with { Y = pos.Y + Size }, StrokeWidth, Color.Black);
        }
    }
}