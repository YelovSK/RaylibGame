using System.Numerics;
using Engine.Components;
using Engine.Extensions;
using Raylib_CSharp;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;

namespace Engine.Systems.Render;

public sealed class ButtonRenderSystem : IRenderSystem
{
    private const float FOV = 100f;

    public void Draw(World world)
    {
        var visuals = world.GetPool<ButtonVisualStateComponent>();
        world.View<RectTransform, ButtonStyleComponent, UiPointerStateComponent>()
            .ForEach((entity, ref transform, ref style, ref interaction) =>
            {
                var color =
                    interaction is { IsPressed: true, IsHovered: true } ? style.PressedColor :
                    interaction.IsHovered ? style.HoverColor :
                    style.NormalColor;

                Span<Vector3> corners =
                [
                    new(-transform.Size.X / 2, -transform.Size.Y / 2, 0),
                    new( transform.Size.X / 2, -transform.Size.Y / 2, 0),
                    new(-transform.Size.X / 2,  transform.Size.Y / 2, 0),
                    new( transform.Size.X / 2,  transform.Size.Y / 2, 0),
                ];
                
                var hasVisual = visuals.TryGet(entity, out var visual);
                var tiltX = hasVisual ? visual.TiltX : 0f;
                var tiltY = hasVisual ? visual.TiltY : 0f;

                var tx = tiltX * RayMath.Deg2Rad;
                var ty = tiltY * RayMath.Deg2Rad;

                for (int i = 0; i < corners.Length; i++)
                {
                    corners[i] = RayMath.Vector3RotateByAxisAngle(
                        corners[i],
                        Vector3.UnitX,
                        tx
                    );
                    corners[i] = RayMath.Vector3RotateByAxisAngle(
                        corners[i],
                        Vector3.UnitY,
                        ty
                    );
                }

                var center = transform.Position + (transform.Size / 2);

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

                Span<Vector2> outline =
                [
                    projected[0],
                    projected[1],
                    projected[3],
                    projected[2],
                    projected[0]
                ];

                Graphics.DrawSplineLinear(outline, style.StrokeWidth, Color.Black);
            });
    }
}