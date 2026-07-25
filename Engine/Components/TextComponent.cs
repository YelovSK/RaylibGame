
using Raylib_cs;

namespace Engine.Components;

public class TextComponent : UiComponent
{
    public string Text;
    public int FontSize = 20;

    public float TextSize()
    {
        return Raylib.MeasureText(Text, FontSize);
    }

    public override void Draw()
    {
        var position = Rect.Bounds.Position;
        Raylib.DrawText(Text, (int)position.X, (int)position.Y, FontSize, Color.White);
    }
}
