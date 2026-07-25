using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;

namespace Engine.Components;

public class TextComponent : UiComponent
{
    public string Text;
    public int FontSize = 20;

    public float TextSize()
    {
        return TextManager.MeasureText(Text, FontSize);
    }

    public override void Draw()
    {
        var position = Rect.Bounds.Position;
        Graphics.DrawText(Text, (int)position.X, (int)position.Y, FontSize, Color.White);
    }
}
