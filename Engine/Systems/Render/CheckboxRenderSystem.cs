using Engine.Components;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;

namespace Engine.Systems.Render;

public class CheckboxRenderSystem : IRenderSystem
{
    public void Draw(World world)
    {
        world.View<RectTransform, CheckboxComponent>().ForEach((entity, ref transform, ref checkbox) =>
        {
            Graphics.DrawRectangleLines((int)transform.Position.X, (int)transform.Position.Y, (int)transform.Size.X, (int)transform.Size.Y, Color.Black);
            if (checkbox.IsChecked)
            {
                Graphics.DrawLine(
                    (int)transform.Position.X,
                    (int)transform.Position.Y,
                    (int)transform.Position.X + (int)transform.Size.X,
                    (int)transform.Position.Y + (int)transform.Size.Y,
                    Color.Black);
                
                Graphics.DrawLine(
                    (int)transform.Position.X + (int)transform.Size.X,
                    (int)transform.Position.Y,
                    (int)transform.Position.X,
                    (int)transform.Position.Y + (int)transform.Size.Y,
                    Color.Black);
            }
        });
    }
}