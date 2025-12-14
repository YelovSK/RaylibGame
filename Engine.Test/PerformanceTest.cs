using Engine.Attributes;

namespace Engine.Test;

abstract internal class TestSystemBase : ISystem
{
    public void Update(World world, float dt) { }
}

internal sealed class ASystem : TestSystemBase { }

[UpdateAfter(typeof(ASystem))]
internal sealed class BAfterA : TestSystemBase { }

[UpdateAfter(typeof(BAfterA))]
internal sealed class CAfterB : TestSystemBase { }

[UpdateBefore(typeof(CBeforeNothing))]
internal sealed class ABeforeC : TestSystemBase { }

internal sealed class CBeforeNothing : TestSystemBase { }

[UpdateAfter(typeof(ABeforeC))]
[UpdateBefore(typeof(CBeforeNothing))]
internal sealed class BBetweenAAndC : TestSystemBase { }

internal sealed class NotInList : TestSystemBase { }

[UpdateAfter(typeof(NotInList))]
internal sealed class AfterMissingConstraint : TestSystemBase { }

[UpdateAfter(typeof(CycleB))]
internal sealed class CycleA : TestSystemBase { }

[UpdateAfter(typeof(CycleA))]
internal sealed class CycleB : TestSystemBase { }

[UpdateAfter(typeof(SelfCycle))]
internal sealed class SelfCycle : TestSystemBase { }

[TestFixture]
public class Tests
{
    private static List<ISystem> Sort(params ISystem[] systems) => SystemScheduler.Build(systems.ToList());
    
    private static int IndexOfType(IReadOnlyList<ISystem> list, Type t)
    {
        for (var i = 0; i < list.Count; i++)
        {
            if (list[i].GetType() == t) return i;
        }

        return -1;
    }

    [Test]
    public void UpdateAfter_ProducesExpectedOrder_ForChain()
    {
        var sorted = Sort(new CAfterB(), new BAfterA(), new ASystem());
        var types = sorted.Select(s => s.GetType()).ToArray();

        Assert.That(types, Is.EqualTo(new[] { typeof(ASystem), typeof(BAfterA), typeof(CAfterB) }));
    }

    [Test]
    public void UpdateBefore_ProducesExpectedOrder()
    {
        var sorted = Sort(new CBeforeNothing(), new ABeforeC());
        var types = sorted.Select(s => s.GetType()).ToArray();

        Assert.That(types, Is.EqualTo(new[] { typeof(ABeforeC), typeof(CBeforeNothing) }));
    }

    [Test]
    public void MixedBeforeAfter_ProducesExpectedOrder()
    {
        var sorted = Sort(new CBeforeNothing(), new BBetweenAAndC(), new ABeforeC());
        var types = sorted.Select(s => s.GetType()).ToArray();

        Assert.That(types, Is.EqualTo(new[] { typeof(ABeforeC), typeof(BBetweenAAndC), typeof(CBeforeNothing) }));
    }

    [Test]
    public void MissingConstraints_AreIgnored()
    {
        var sorted = Sort(new AfterMissingConstraint());
        Assert.That(sorted.Count, Is.EqualTo(1));
        Assert.That(sorted[0], Is.TypeOf<AfterMissingConstraint>());
    }

    [Test]
    public void Cycle_ThrowsInvalidOperationException()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Sort(new CycleA(), new CycleB());
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex!.Message, Does.Contain("cycle"));
            Assert.That(ex.Message, Does.Contain(typeof(CycleA).FullName));
        }
        Assert.That(ex.Message, Does.Contain(typeof(CycleB).FullName));
    }

    [Test]
    public void SelfCycle_ThrowsInvalidOperationException()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Sort(new SelfCycle());
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex!.Message, Does.Contain("cycle"));
            Assert.That(ex.Message, Does.Contain(typeof(SelfCycle).FullName));
        }
    }

    [Test]
    public void DuplicateSystemType_ThrowsInvalidOperationException()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            Sort(new ASystem(), new ASystem());
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex!.Message, Does.Contain("Duplicate system type"));
            Assert.That(ex.Message, Does.Contain(typeof(ASystem).FullName));
        }
    }

    [Test]
    public void NoConstraints_ReturnsAllSystems()
    {
        // Order is not guaranteed here; just validate membership.
        var s1 = new ASystem();
        var s2 = new CBeforeNothing();
        var s3 = new NotInList();

        var sorted = Sort(s1, s2, s3);
        var types = sorted.Select(s => s.GetType()).ToHashSet();

        Assert.That(types.SetEquals([typeof(ASystem), typeof(CBeforeNothing), typeof(NotInList)]), Is.True);
    }
}