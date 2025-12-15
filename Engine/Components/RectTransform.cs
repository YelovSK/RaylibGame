using Raylib_CSharp.Transformations;

namespace Engine.Components;

public class RectTransform : Component, IUpdatable
{
    public Rectangle Rectangle;
    
    public void Update(float dt)
    {
        Entity.Transform.Position = Rectangle.Position;
    }
}