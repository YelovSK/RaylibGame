using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;

namespace Engine.Components;

public class TextComponent : Component
{
    public required string Text;
    public int FontSize = 20;

    public float TextSize()
    {
        return TextManager.MeasureText(Text, FontSize);
    }

    public override void Draw()
    {
        Graphics.DrawText(Text,
            (int)Entity.Transform.Position.X,
            (int)Entity.Transform.Position.Y,
            FontSize,
            Color.White);
    }
}