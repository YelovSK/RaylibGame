using Engine.Enums;

namespace Engine.Components;

public abstract class UiComponent : Component, IDrawable
{
    public RenderSpace RenderSpace { get; set; } = RenderSpace.Screen;
    public RectTransform Rect { get; } = new();

    public abstract void Draw();
}
