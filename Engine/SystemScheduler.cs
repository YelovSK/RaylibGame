using System.Reflection;
using Engine.Attributes;

namespace Engine;

public static class SystemScheduler
{
    public static List<T> Build<T>(List<T> systems) => TopoSort(systems);

    private static List<T> TopoSort<T>(List<T> systems)
    {
        // Assumption: one instance per system type per group.
        // If you want multiple instances of same type, you’ll need a different key.
        var byType = new Dictionary<Type, T>(systems.Count);
        foreach (var s in systems)
        {
            var t = s.GetType();
            if (!byType.TryAdd(t, s))
            {
                throw new InvalidOperationException(
                    $"Duplicate system type in the same group: {t.FullName}"
                );
            }
        }

        var nodes = byType.Keys.ToArray();

        var outEdges = new Dictionary<Type, HashSet<Type>>(nodes.Length);
        var inEdges = new Dictionary<Type, HashSet<Type>>(nodes.Length);
        var indegree = new Dictionary<Type, int>(nodes.Length);

        foreach (var n in nodes)
        {
            outEdges[n] = new HashSet<Type>();
            inEdges[n] = new HashSet<Type>();
            indegree[n] = 0;
        }

        void AddEdge(Type from, Type to)
        {
            if (!outEdges[from].Add(to)) return; // already exists
            inEdges[to].Add(from);
            indegree[to] = indegree[to] + 1;
        }

        foreach (var s in systems)
        {
            var a = s.GetType();

            // A after B  => B -> A
            var after = a.GetCustomAttributes<UpdateAfterAttribute>(inherit: true);
            foreach (var attr in after)
            {
                var b = attr.Other;
                if (!byType.ContainsKey(b)) continue; // constraint outside group
                AddEdge(b, a);
            }

            // A before B => A -> B
            var before = a.GetCustomAttributes<UpdateBeforeAttribute>(inherit: true);
            foreach (var attr in before)
            {
                var b = attr.Other;
                if (!byType.ContainsKey(b)) continue; // constraint outside group
                AddEdge(a, b);
            }
        }

        // Kahn’s algorithm
        var queue = new Queue<Type>(nodes.Length);
        foreach (var n in nodes)
        {
            if (indegree[n] == 0) queue.Enqueue(n);
        }

        var sorted = new List<T>(nodes.Length);

        while (queue.Count > 0)
        {
            var n = queue.Dequeue();
            sorted.Add(byType[n]);

            foreach (var m in outEdges[n])
            {
                indegree[m] = indegree[m] - 1;
                if (indegree[m] == 0) queue.Enqueue(m);
            }
        }

        if (sorted.Count != nodes.Length)
        {
            // Cycle (or unresolved constraints among remaining nodes)
            var remaining = nodes
                .Where(t => indegree[t] > 0)
                .OrderBy(t => t.FullName)
                .ToArray();

            var lines = new List<string>
            {
                "System ordering contains a cycle. Remaining nodes:"
            };

            foreach (var t in remaining)
            {
                var preds = inEdges[t]
                    .Where(p => indegree[p] > 0)
                    .Select(p => p.FullName)
                    .OrderBy(n => n)
                    .ToArray();

                lines.Add(
                    $"- {t.FullName} (waiting for: {string.Join(", ", preds)})"
                );
            }

            throw new InvalidOperationException(string.Join("\n", lines));
        }

        return sorted;
    }
}