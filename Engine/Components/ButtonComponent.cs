using System.Numerics;
using Engine.Extensions;
using Raylib_CSharp;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;

namespace Engine.Components;

public class ButtonComponent : UiControlComponent
{
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

    protected override void UpdateControl(float dt, Vector2 mousePosition)
    {
        HandleTilt(dt, Rect.Bounds, mousePosition);
    }

    protected override void Click()
    {
        OnClick?.Invoke();
    }

    private void HandleTilt(float dt, Rectangle bounds, Vector2 mousePosition)
    {
        var center = bounds.Center();
        var offset = center - mousePosition;

        float targetTiltX;
        float targetTiltY;

        if (!IsHovered)
        {
            targetTiltX = 0;
            targetTiltY = 0;
        }
        else
        {
            var normalizedX = Math.Clamp(offset.X / (bounds.Width / 2), -1f, 1f);
            var normalizedY = Math.Clamp(offset.Y / (bounds.Height / 2), -1f, 1f);

            targetTiltX = -normalizedY * MAX_TILT_DEGREES;
            targetTiltY = normalizedX * MAX_TILT_DEGREES;
        }

        _currentTiltX = RayMath.Lerp(_currentTiltX, targetTiltX, TILT_SPEED * dt);
        _currentTiltY = RayMath.Lerp(_currentTiltY, targetTiltY, TILT_SPEED * dt);
    }

    public override void Draw()
    {
        var bounds = Rect.Bounds;
        var color = State switch
        {
            InteractableState.Normal => NormalColor,
            InteractableState.Hovered => HoverColor,
            InteractableState.Pressed => PressedColor,
            _ => NormalColor,
        };

        Span<Vector3> corners =
        [
            new(-bounds.Width / 2, -bounds.Height / 2, 0),
            new( bounds.Width / 2, -bounds.Height / 2, 0),
            new(-bounds.Width / 2,  bounds.Height / 2, 0),
            new( bounds.Width / 2,  bounds.Height / 2, 0),
        ];

        var tiltXRad = _currentTiltX * RayMath.Deg2Rad;
        var tiltYRad = _currentTiltY * RayMath.Deg2Rad;

        for (var i = 0; i < corners.Length; i++)
        {
            corners[i] = RayMath.Vector3RotateByAxisAngle(corners[i], Vector3.UnitX, tiltXRad);
            corners[i] = RayMath.Vector3RotateByAxisAngle(corners[i], Vector3.UnitY, tiltYRad);
        }

        var center = bounds.Center();
        Span<Vector2> projected = stackalloc Vector2[4];
        for (var i = 0; i < corners.Length; i++)
        {
            var scale = FOV / (FOV + corners[i].Z);
            projected[i] = new Vector2(
                center.X + corners[i].X * scale,
                center.Y + corners[i].Y * scale
            );
        }

        Graphics.DrawQuad(projected[0], projected[1], projected[2], projected[3], color);

        Span<Vector2> outline = [projected[0], projected[1], projected[3], projected[2], projected[0]];
        Graphics.DrawSplineLinear(outline, StrokeWidth, Color.Black);

        var textSize = TextManager.MeasureText(Text, FontSize);
        var textPosition = new Vector2(
            bounds.X + (bounds.Width - textSize) / 2,
            bounds.Y + (bounds.Height - FontSize) / 2
        );

        if (IsHovered)
        {
            var sin = Math.Sin(2 * Math.PI * Time.GetTime());
            textPosition.Y += (float)(sin * VirtualViewport.Height * 0.01f);
        }

        Graphics.DrawText(Text, (int)textPosition.X, (int)textPosition.Y, FontSize, TextColor);
    }
}
