using System.Numerics;
using Engine.Components;
using Engine.Extensions;
using Raylib_CSharp;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;

namespace Engine.Systems;

public sealed class ButtonInputSystem : ISystem
{
    private const float MaxTilt = 15f;
    private const float TiltSpeed = 10f;

    public void Update(World world, float dt)
    {
        var mousePos = Input.GetVirtualMousePosition();

        world.View<ButtonComponent, TransformComponent, ButtonStateComponent>()
             .ForEach((_, ref buttonComponent, ref transformComponent, ref stateComponent) =>
             {
                 var bounds = new Rectangle(
                     transformComponent.Position.X,
                     transformComponent.Position.Y,
                     buttonComponent.Size.X,
                     buttonComponent.Size.Y
                 );

                 stateComponent.IsHovered =
                     ShapeHelper.CheckCollisionPointRec(mousePos, bounds);

                 HandleTilt(dt, buttonComponent.Size, bounds, mousePos, ref stateComponent);

                 if (stateComponent.IsHovered && Input.IsMouseButtonPressed(MouseButton.Left))
                     stateComponent.WasPressed = true;

                 if (stateComponent.WasPressed && Input.IsMouseButtonReleased(MouseButton.Left))
                 {
                     if (stateComponent.IsHovered)
                         //button.OnClick?.Invoke();

                         stateComponent.WasPressed = false;
                 }
             });
    }

    private static void HandleTilt(
        float dt,
        Vector2 size,
        Rectangle bounds,
        Vector2 mousePos,
        ref ButtonStateComponent stateComponent)
    {
        float targetX = 0;
        float targetY = 0;

        if (stateComponent.IsHovered)
        {
            var center = bounds.Center();
            var offset = center - mousePos;

            var nx = Math.Clamp(offset.X / (size.X / 2), -1f, 1f);
            var ny = Math.Clamp(offset.Y / (size.Y / 2), -1f, 1f);

            targetX = -ny * MaxTilt;
            targetY = nx * MaxTilt;
        }

        stateComponent.TiltX = RayMath.Lerp(stateComponent.TiltX, targetX, TiltSpeed * dt);
        stateComponent.TiltY = RayMath.Lerp(stateComponent.TiltY, targetY, TiltSpeed * dt);
    }
}