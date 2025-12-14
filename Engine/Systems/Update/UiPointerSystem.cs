using Engine.Components;
using Engine.Extensions;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;

namespace Engine.Systems.Update;

public sealed class UiPointerSystem : ISystem
{
    public void Update(World world, float dt)
    {
        var mousePos = Input.GetVirtualMousePosition();
        
        world.View<RectTransform, UiPointerStateComponent>().ForEach((entity, ref transform, ref state) =>
        {
            state.IsClicked = false;
            
            var bounds = new Rectangle(
                transform.Position.X,
                transform.Position.Y,
                transform.Size.X,
                transform.Size.Y
            );

            state.IsHovered = ShapeHelper.CheckCollisionPointRec(mousePos, bounds);

            // Press starts only if mouse pressed while hovered
            if (state.IsHovered &&
                Input.IsMouseButtonPressed(MouseButton.Left))
            {
                state.IsPressed = true;
            }

            // Release: if we had captured press, and still hovered => click
            if (state.IsPressed &&
                Input.IsMouseButtonReleased(MouseButton.Left))
            {
                if (state.IsHovered)
                {
                    state.IsClicked = true;
                    state.Action?.Invoke();
                }

                state.IsPressed = false;
            }
        });
    }
}