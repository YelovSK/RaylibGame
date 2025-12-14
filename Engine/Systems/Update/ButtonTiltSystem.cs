using System.Numerics;
using Engine.Attributes;
using Engine.Components;
using Engine.Extensions;
using Raylib_CSharp;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;

namespace Engine.Systems.Update;

[UpdateAfter(typeof(UiPointerSystem))]
public sealed class ButtonTiltSystem : ISystem
{
    private const float MaxTilt = 15f;
    private const float TiltSpeed = 10f;

    public void Update(World world, float dt)
    {
        var job = new Job { MousePosition = Input.GetVirtualMousePosition(), Dt = dt };
        var view = world.View<RectTransform, UiPointerStateComponent, ButtonVisualStateComponent>();
        view.ExecuteJob(ref job);
    }

    private struct Job : IForEachJob<RectTransform, UiPointerStateComponent, ButtonVisualStateComponent>
    {
        public float Dt;
        public Vector2 MousePosition;

        public void Execute(Entity entity, ref RectTransform transform, ref UiPointerStateComponent interaction, ref ButtonVisualStateComponent visual)
        {
            var bounds = new Rectangle(
                transform.Position.X,
                transform.Position.Y,
                transform.Size.X,
                transform.Size.Y
            );
                
            float targetX = 0;
            float targetY = 0;

            if (interaction.IsHovered)
            {
                var center = bounds.Center();
                var offset = center - MousePosition;

                var nx = Math.Clamp(offset.X / (transform.Size.X / 2), -1f, 1f);
                var ny = Math.Clamp(offset.Y / (transform.Size.Y / 2), -1f, 1f);

                targetX = -ny * MaxTilt;
                targetY = nx * MaxTilt;
            }

            visual.TiltX = RayMath.Lerp(visual.TiltX, targetX, TiltSpeed * Dt);
            visual.TiltY = RayMath.Lerp(visual.TiltY, targetY, TiltSpeed * Dt);
        }
    }
}