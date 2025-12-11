namespace Engine;

/// <summary>
/// IReadOnlyList and ReadOnlyCollection cause allocations when using foreach because of boxing.
/// </summary>
public readonly struct ReadOnlyList<T>(List<T> list)
{
    public int Count => list.Count;
    public T this[int index] => list[index];

    public List<T>.Enumerator GetEnumerator() => list.GetEnumerator();
    // Can be cast back to list, but who cares.
    public IEnumerable<T> AsEnumerable() => list;
}