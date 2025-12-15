using System.Numerics;
using Engine.Enums;
using Engine.Extensions;
using Raylib_CSharp;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;

namespace Engine.Components;

public class ButtonComponent : Component, IUpdatable, IDrawable
{
    public RenderSpace RenderSpace { get; set; } = RenderSpace.Screen;
    
    public string Text;
    public int FontSize = 20;
    public float StrokeWidth = 2f;
    public Color NormalColor = Color.DarkGray;
    public Color HoverColor = Color.Gray;
    public Color PressedColor = Color.LightGray;
    public Color TextColor = Color.White;
    
    public Action? OnClick;

    private float _currentTiltX;
    private float _currentTiltY;
    
    private const float MAX_TILT_DEGREES = 15f;
    private const float TILT_SPEED = 10f;
    private const float FOV = 100f;

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
        
        HandleTilt(dt, _rectTransform.Rectangle, Input.GetVirtualMousePosition());

        if (_guiInteractableComponent.IsClicked)
        {
            OnClick?.Invoke();
        }
    }

    private void HandleTilt(float dt, Rectangle bounds, Vector2 mousePos)
    {
        var center = bounds.Center();
        var offset = center - mousePos;

        float targetTiltX;
        float targetTiltY;
        
        if (!_guiInteractableComponent?.IsHovered ?? false)
        {
            targetTiltX = 0;
            targetTiltY = 0;
        }
        else
        {
            var normalizedX = Math.Clamp(offset.X / (_rectTransform!.Rectangle.Width / 2), -1f, 1f);
            var normalizedY = Math.Clamp(offset.Y / (_rectTransform!.Rectangle.Height / 2), -1f, 1f);
        
            targetTiltX = -normalizedY * MAX_TILT_DEGREES;
            targetTiltY = normalizedX * MAX_TILT_DEGREES;
        }
        
        _currentTiltX = RayMath.Lerp(_currentTiltX, targetTiltX, TILT_SPEED * dt);
        _currentTiltY = RayMath.Lerp(_currentTiltY, targetTiltY, TILT_SPEED * dt);
    }
    
    public void Draw()
    {
        var color = _guiInteractableComponent?.State switch
        {
            InteractableState.Normal => NormalColor,
            InteractableState.Hoverered => HoverColor,
            InteractableState.Pressed => PressedColor,
            _ => NormalColor,
        };
        
        // Tilt the button in 3D space and project it into 2D
        Span<Vector3> corners =
        [
            new(-_rectTransform.Rectangle.Width/2, -_rectTransform.Rectangle.Height/2, 0),
            new( _rectTransform.Rectangle.Width/2, -_rectTransform.Rectangle.Height/2, 0),
            new(-_rectTransform.Rectangle.Width/2,  _rectTransform.Rectangle.Height/2, 0),
            new( _rectTransform.Rectangle.Width/2,  _rectTransform.Rectangle.Height/2, 0),
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

        var center = _rectTransform.Rectangle.Center();
        
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
            _rectTransform.Rectangle.X + (_rectTransform.Rectangle.Width - textSize) / 2,
            _rectTransform.Rectangle.Y + (_rectTransform.Rectangle.Height - FontSize) / 2
        );

        // Bounce on it crazy style
        if (_guiInteractableComponent?.IsHovered ?? false)
        {
            var sin = Math.Sin(2 * Math.PI * Time.GetTime());
            textPos.Y += (float)(sin * Application.Instance.VirtualHeight * 0.01f);
        }
        
        Graphics.DrawText(Text, (int)textPos.X, (int)textPos.Y, FontSize, TextColor);
    }
}