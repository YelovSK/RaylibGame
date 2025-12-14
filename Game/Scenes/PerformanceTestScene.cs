using System.Numerics;
using Engine;

namespace Game.Scenes;

public class PerformanceTestScene : Scene
{
    private class MovementSystem : ISystem
    {
        public void Update(World world, float dt)
        {
            var job = new Job { Dt = dt };
            world.View<Comp0, Comp1, Comp2, Comp3, Comp4>().ExecuteJob(ref job);
        }

        private struct Job : IForEachJob<Comp0, Comp1, Comp2, Comp3, Comp4>
        {
            public float Dt;
            public void Execute(Entity entity, ref Comp0 c1, ref Comp1 c2, ref Comp2 c3, ref Comp3 c4, ref Comp4 c5)
            {
                c1.A += Dt;
            }
        }
    }

    private struct Comp0 { public Vector2 Vec; public float A; public long B; }
    private struct Comp1 { public Vector2 Vec; }
    private struct Comp2 { public Vector2 Vec; }
    private struct Comp3 { public Vector2 Vec; }
    private struct Comp4 { public Vector2 Vec; }
    private struct Comp5 { public Vector2 Vec; }
    
    protected override void Load(World world)
    {
        for (var i = 0; i < 100_000; i++)
        {
            var entity = world.CreateEntity();
            world.AddComponent(entity, new Comp0());
            world.AddComponent(entity, new Comp1());
            world.AddComponent(entity, new Comp2());
            world.AddComponent(entity, new Comp3());
            world.AddComponent(entity, new Comp4());
            world.AddComponent(entity, new Comp5());
        }
        
        world.AddSystem<MovementSystem>();
    }
}