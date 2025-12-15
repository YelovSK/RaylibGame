using Engine.Enums;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;

namespace Engine.Components;

/// <summary>
/// Screen-space rectangle. Requires RectTransform.
/// </summary>
public class RectangleComponent : Component, IDrawable
{
    public RenderSpace RenderSpace { get; set; } = RenderSpace.Screen;
    public Color Color = Color.Green;

    private RectTransform? _rectTransform;

    public override void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Draw()
    {
        if (_rectTransform != null)
        {
            Graphics.DrawRectangleRec(_rectTransform.Rectangle, Color);
        }
    }
}
