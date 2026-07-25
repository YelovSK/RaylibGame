using Engine.Enums;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;

namespace Engine.Components;

public class TextComponent : Component, IDrawable
{
    public RenderSpace RenderSpace { get; set; } = RenderSpace.Screen;
    
    public string Text;
    public int FontSize = 20;

    private RectTransform? _rectTransform;

    public override void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public float TextSize()
    {
        return TextManager.MeasureText(Text, FontSize);
    }

    public void Draw()
    {
        if (_rectTransform is null)
        {
            return;
        }

        var position = _rectTransform.Bounds.Position;
        Graphics.DrawText(Text,
            (int)position.X,
            (int)position.Y,
            FontSize,
            Color.White);
    }
}
