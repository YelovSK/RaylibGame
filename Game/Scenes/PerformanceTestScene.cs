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

        for (var i = 0; i < 100_000; i++)
        {
            var r = Convert.ToByte(Random.Shared.Next(0, 255));
            var g = Convert.ToByte(Random.Shared.Next(0, 255));
            var b = Convert.ToByte(Random.Shared.Next(0, 255));
            var entity = CreateEntity();
            entity.Transform.Position = middle;
            var sprite = entity.AddComponent<SpriteComponent>();
            sprite.Width = 10;
            sprite.Height = 10;
            sprite.Color = new Color(r, g, b, 255);
            entity.AddComponent<RandomMovementComponent>();
        }
    }
}

public class RandomMovementComponent : Component, IUpdatable
{
    private readonly Random _random = new();
    
    public void Update(float dt)
    {
        var dx = _random.Next(0, 10);
        var dy = _random.Next(0, 10);
        Entity.Transform.Position += new Vector2(dx, dy) * dt;
    }
}