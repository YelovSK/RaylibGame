namespace Engine;

/// <summary>
/// IReadOnlyList and ReadOnlyCollection cause allocations when using foreach because of boxing.
/// </summary>
public readonly struct ReadOnlyCollections<T>(List<T> list)
{
    public int Count => list.Count;
    public T this[int index] => list[index];

    public List<T>.Enumerator GetEnumerator() => list.GetEnumerator();
    // Can be cast back to list, but who cares.
    public IEnumerable<T> AsEnumerable() => list;
}

/// <summary>
/// IReadOnlySet and ReadOnlyCollection cause allocations when using foreach because of boxing.
/// </summary>
public readonly struct ReadOnlyHashSet<T>(HashSet<T> set)
{
    public int Count => set.Count;

    public HashSet<T>.Enumerator GetEnumerator() => set.GetEnumerator();
    // Can be cast back to hashset, but who cares.
    public IEnumerable<T> AsEnumerable() => set;
}
