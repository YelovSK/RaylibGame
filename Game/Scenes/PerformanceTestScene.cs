using System.Numerics;
using Engine;
using Engine.Components;
using Engine.Helpers;
using Raylib_CSharp.Colors;

namespace Game.Scenes;

public class PerformanceTestScene : Scene
{
    public override void Load()
    {
        var middle = VirtualLayout.Center();
        for (int i = 0; i < 10_000; i++)
        {
            var r = Convert.ToByte(Random.Shared.Next(0, 255));
            var g = Convert.ToByte(Random.Shared.Next(0, 255));
            var b = Convert.ToByte(Random.Shared.Next(0, 255));
            var entity = new Entity(middle);
            entity.AddComponent(new SpriteComponent()
            {
                Width = 10,
                Height = 10,
                Color = new Color(r, g, b, 255)
            });
            entity.AddComponent(new RandomMovementComponent());
            AddEntity(entity);
        }
    }
}

public class RandomMovementComponent : Component
{
    private Random _random = new();
    public override void Update(float dt)
    {
        var dx = _random.Next(0, 10);
        var dy = _random.Next(0, 10);
        Entity.Transform.Position += new Vector2(dx, dy) * dt;
    }
}