using System.Numerics;
using Engine.Extensions;
using Raylib_CSharp;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;

namespace Engine.Components;

public class ButtonComponent : Component
{
    public required string Text;
    public int FontSize = 20;
    public Vector2 Size;
    public float StrokeWidth = 2f;
    public Color NormalColor = Color.DarkGray;
    public Color HoverColor = Color.Gray;
    public Color PressedColor = Color.LightGray;
    public Color TextColor = Color.White;
    
    public Action? OnClick;

    private bool _isHovered;
    private bool _wasPressed;

    private float _currentTiltX;
    private float _currentTiltY;
    
    private const float MAX_TILT_DEGREES = 15f;
    private const float TILT_SPEED = 10f;
    private const float FOV = 100f;

    public override void Update(float dt)
    {
        var mousePos = Input.GetVirtualMousePosition();
        var bounds = new Rectangle(
            Entity.Transform.Position.X,
            Entity.Transform.Position.Y,
            Size.X,
            Size.Y
        );

        _isHovered = ShapeHelper.CheckCollisionPointRec(mousePos, bounds);
        
        HandleTilt(dt, bounds, mousePos);

        if (_isHovered && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            _wasPressed = true;
        }

        if (_wasPressed && Input.IsMouseButtonReleased(MouseButton.Left))
        {
            if (_isHovered)
            {
                OnClick?.Invoke();
            }

            _wasPressed = false;
        }
    }

    private void HandleTilt(float dt, Rectangle bounds, Vector2 mousePos)
    {
        var center = bounds.Center();
        var offset = center - mousePos;

        float targetTiltX;
        float targetTiltY;
        
        if (!_isHovered)
        {
            targetTiltX = 0;
            targetTiltY = 0;
        }
        else
        {
            var normalizedX = Math.Clamp(offset.X / (Size.X / 2), -1f, 1f);
            var normalizedY = Math.Clamp(offset.Y / (Size.Y / 2), -1f, 1f);
        
            targetTiltX = -normalizedY * MAX_TILT_DEGREES;
            targetTiltY = normalizedX * MAX_TILT_DEGREES;
        }
        
        _currentTiltX = RayMath.Lerp(_currentTiltX, targetTiltX, TILT_SPEED * dt);
        _currentTiltY = RayMath.Lerp(_currentTiltY, targetTiltY, TILT_SPEED * dt);
    }

    public override void Draw()
    {
        var color = GetBackgroundColor();
        
        // Tilt the button in 3D space and project it into 2D
        Span<Vector3> corners =
        [
            new(-Size.X/2, -Size.Y/2, 0),
            new( Size.X/2, -Size.Y/2, 0),
            new(-Size.X/2,  Size.Y/2, 0),
            new( Size.X/2,  Size.Y/2, 0),
        ];
        
        var tiltXRad = _currentTiltX * RayMath.Deg2Rad;
        var tiltYRad = _currentTiltY * RayMath.Deg2Rad;
        
        for (var i = 0; i < corners.Length; i++)
        {
            corners[i] = RayMath.Vector3RotateByAxisAngle(
                corners[i], 
                new Vector3(1, 0, 0),
                tiltXRad
            );
            corners[i] = RayMath.Vector3RotateByAxisAngle(
                corners[i], 
                new Vector3(0, 1, 0),
                tiltYRad
            );
        }

        var center = Entity.Transform.Position + (Size / 2);
        
        // TODO: i think this could be a helper
        Span<Vector2> projected = stackalloc Vector2[4];
        for (var i = 0; i < corners.Length; i++)
        {
            var scale = FOV / (FOV + corners[i].Z);
            projected[i] = new Vector2(
                center.X + corners[i].X * scale,
                center.Y + corners[i].Y * scale
            );
        }
        
        // Background
        Graphics.DrawQuad(projected[0], projected[1], projected[2], projected[3], color);
        
        // Outline
        Span<Vector2> outlinesPoints = [ projected[0], projected[1], projected[3], projected[2], projected[0] ];
        Graphics.DrawSplineLinear(outlinesPoints, StrokeWidth, Color.Black);

        // Text
        var textSize = TextManager.MeasureText(Text, FontSize);
        var textPos = new Vector2(
            Entity.Transform.Position.X + (Size.X - textSize) / 2,
            Entity.Transform.Position.Y + (Size.Y - FontSize) / 2
        );

        // Bounce on it crazy style
        if (_isHovered)
        {
            var sin = Math.Sin(2 * Math.PI * Time.GetTime());
            textPos.Y += (float)(sin * Application.Instance.VirtualHeight * 0.01f);
        }
        
        Graphics.DrawText(Text, (int)textPos.X, (int)textPos.Y, FontSize, TextColor);
    }

    private Color GetBackgroundColor()
    {
        if (_wasPressed && _isHovered)
        {
            return PressedColor;
        }
        
        if (_isHovered)
        {
            return HoverColor;
        }

        return NormalColor;
    }
}