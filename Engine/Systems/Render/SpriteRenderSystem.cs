using Engine.Components;
using Raylib_CSharp.Rendering;

namespace Engine.Systems.Render;

public sealed class SpriteRenderSystem : IRenderSystem
{
    public void Draw(World world)
    {
        world.View<TransformComponent, SpriteComponent>()
            .ForEach((_, ref transformComponent, ref rect) =>
            {
                Graphics.DrawRectangleV(transformComponent.Position, rect.Size, rect.Color);
            });
    }
}