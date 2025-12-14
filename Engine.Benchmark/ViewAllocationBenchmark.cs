using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace Engine.Benchmark;

[ShortRunJob]
[MemoryDiagnoser]
public class ViewAllocationBenchmark
{
    private World _world;
    
    [GlobalSetup]
    public void GlobalSetup()
    {
        _world = new World();

        for (var i = 0; i < 1_000_000; i++)
        {
            var e = _world.CreateEntity();

            _world.AddComponent(e, new CompA());
            _world.AddComponent(e, new CompB());
            _world.AddComponent(e, new CompC());
            _world.AddComponent(e, new CompD());
            _world.AddComponent(e, new CompE());
            
            //if (i % 2 == 0) _world.AddComponent(e, new CompB());
            //if (i % 3 == 0) _world.AddComponent(e, new CompC());
            //if (i % 4 == 0) _world.AddComponent(e, new CompD());
            //if (i % 5 == 0) _world.AddComponent(e, new CompE());
        }
    }

    // Closure allocation
    [Benchmark]
    public void WithAllocation()
    {
        var data = Vector2.One;
        _world.View<CompA, CompB, CompC, CompD, CompE>()
            .ForEach((_, ref a, ref _, ref _, ref _, ref _) => { a.A += data; });
    }
    
    // No closure allocation because nothing is passed in the delegate.
    [Benchmark]
    public void WithoutAllocation()
    {
        _world.View<CompA, CompB, CompC, CompD, CompE>()
            .ForEach((_, ref a, ref _, ref _, ref _, ref _) => { a.A += Vector2.One; });
    }
    
    // Struct job, something like what Unity DOTS does
    [Benchmark]
    public void WithoutAllocationStructJob()
    {
        var job = new AddVectorJob {Data = Vector2.One};
        _world.View<CompA, CompB, CompC, CompD, CompE>().ExecuteJob(ref job);
    }
}

struct AddVectorJob : IForEachJob<CompA, CompB, CompC, CompD, CompE>
{
    public Vector2 Data;
    
    public void Execute(Entity entity, ref CompA a, ref CompB b, ref CompC c, 
        ref CompD d, ref CompE e)
    {
        a.A += Data;
    }
}