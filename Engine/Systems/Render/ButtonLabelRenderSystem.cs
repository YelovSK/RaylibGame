using System.Numerics;
using Engine.Attributes;
using Engine.Components;
using Raylib_CSharp;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;

namespace Engine.Systems.Render;

[UpdateAfter(typeof(ButtonRenderSystem))]
public sealed class ButtonLabelRenderSystem : IRenderSystem
{
    public void Draw(World world)
    {
        world.View<RectTransform, LabelComponent, UiPointerStateComponent>()
            .ForEach((_, ref transform, ref label, ref interaction) =>
            {
                var width = TextManager.MeasureText(label.Text, label.FontSize);

                var pos = new Vector2(
                    transform.Position.X + (transform.Size.X - width) / 2,
                    transform.Position.Y + (transform.Size.Y - label.FontSize) / 2
                );

                if (interaction.IsHovered)
                {
                    var sin = Math.Sin(2 * Math.PI * Time.GetTime());
                    pos.Y += (float)(sin * Application.Instance.VirtualHeight * 0.01f);
                }

                Graphics.DrawText(
                    label.Text,
                    (int)pos.X,
                    (int)pos.Y,
                    label.FontSize,
                    label.TextColor
                );
            });
    }
}