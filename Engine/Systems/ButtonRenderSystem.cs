using System.Numerics;
using Engine.Components;
using Engine.Extensions;
using Raylib_CSharp;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;

namespace Engine.Systems;

public sealed class ButtonRenderSystem : IRenderSystem
{
    private const float FOV = 100f;

    public void Draw(World world)
    {
        world.View<ButtonComponent, TransformComponent, ButtonStateComponent>()
             .ForEach((
                 _,
                 ref buttonComponent,
                 ref transformComponent,
                 ref stateComponent
             ) => DrawButton(buttonComponent, transformComponent, stateComponent));
    }

    private static void DrawButton(
        ButtonComponent buttonComponent,
        TransformComponent transformComponent,
        ButtonStateComponent stateComponent)
    {
        var color =
            stateComponent.WasPressed && stateComponent.IsHovered ? buttonComponent.PressedColor :
            stateComponent.IsHovered ? buttonComponent.HoverColor :
            buttonComponent.NormalColor;

        Span<Vector3> corners =
        [
            new(-buttonComponent.Size.X / 2, -buttonComponent.Size.Y / 2, 0),
            new( buttonComponent.Size.X / 2, -buttonComponent.Size.Y / 2, 0),
            new(-buttonComponent.Size.X / 2,  buttonComponent.Size.Y / 2, 0),
            new( buttonComponent.Size.X / 2,  buttonComponent.Size.Y / 2, 0),
        ];

        var tx = stateComponent.TiltX * RayMath.Deg2Rad;
        var ty = stateComponent.TiltY * RayMath.Deg2Rad;

        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = RayMath.Vector3RotateByAxisAngle(corners[i], Vector3.UnitX, tx);
            corners[i] = RayMath.Vector3RotateByAxisAngle(corners[i], Vector3.UnitY, ty);
        }

        var center = transformComponent.Position + (buttonComponent.Size / 2);

        Span<Vector2> projected = stackalloc Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            var scale = FOV / (FOV + corners[i].Z);
            projected[i] = new Vector2(
                center.X + corners[i].X * scale,
                center.Y + corners[i].Y * scale
            );
        }

        Graphics.DrawQuad(projected[0], projected[1], projected[2], projected[3], color);

        Span<Vector2> outline = [ projected[0], projected[1], projected[3], projected[2], projected[0] ];

        Graphics.DrawSplineLinear(outline, buttonComponent.StrokeWidth, Color.Black);

        DrawText(buttonComponent, transformComponent, stateComponent);
    }

    private static void DrawText(
        ButtonComponent buttonComponent,
        TransformComponent transformComponent,
        ButtonStateComponent stateComponent)
    {
        var size = TextManager.MeasureText(buttonComponent.Text, buttonComponent.FontSize);

        var pos = new Vector2(
            transformComponent.Position.X + (buttonComponent.Size.X - size) / 2,
            transformComponent.Position.Y + (buttonComponent.Size.Y - buttonComponent.FontSize) / 2
        );

        if (stateComponent.IsHovered)
        {
            var sin = Math.Sin(2 * Math.PI * Time.GetTime());
            pos.Y += (float)(sin * Application.Instance.VirtualHeight * 0.01f);
        }

        Graphics.DrawText(
            buttonComponent.Text, (int)pos.X, (int)pos.Y,
            buttonComponent.FontSize, buttonComponent.TextColor);
    }
}