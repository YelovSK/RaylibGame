using BenchmarkDotNet.Attributes;

namespace Engine.Benchmark;

[ShortRunJob]
[MemoryDiagnoser]
public class ViewBenchmark
{
    private World _world;
    private float DT => 0.01f;
    
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

    [Benchmark]
    public void View1()
    {
        _world.View<CompA>()
            .ForEach((_, ref a) => { a.A.X += 10f * DT; });
    }

    [Benchmark]
    public void View2()
        => _world.View<CompA, CompB>()
            .ForEach((_, ref a, ref _) => { a.A.X += 10f * DT; });

    [Benchmark]
    public void View3()
        => _world.View<CompA, CompB, CompC>()
            .ForEach((_, ref a, ref _, ref _) => { a.A.X += 10f * DT; });

    [Benchmark]
    public void View4()
        => _world.View<CompA, CompB, CompC, CompD>()
            .ForEach((_, ref a, ref _, ref _, ref _) => { a.A.X += 10f * DT; });

    [Benchmark]
    public void View5()
        => _world.View<CompA, CompB, CompC, CompD, CompE>()
            .ForEach((_, ref a, ref _, ref _, ref _, ref _) => { a.A.X += 10f * DT; });
}