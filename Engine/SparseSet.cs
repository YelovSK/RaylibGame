using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Engine;

public interface IComponentPool
{
    bool Delete(Entity e);
}

public sealed class SparseSet<T> : IComponentPool where T : struct
{
    private const int SparsePageSize = 1024;
    private const int Tombstone = -1;

    // sparsePages[page][local] = denseIndex or Tombstone
    private readonly List<int[]> _sparsePages = [];

    private readonly List<T> _dense = [];
    private readonly List<Entity> _denseToEntity = [];

    public SparseSet(int initialCapacity = 1024)
    {
        if (initialCapacity > 0)
        {
            _dense.Capacity = initialCapacity;
            _denseToEntity.Capacity = initialCapacity;
        }
    }

    public int Count => _dense.Count;
    public bool IsEmpty => _dense.Count == 0;
    
    public IReadOnlyList<T> DenseData => _dense;
    public IReadOnlyList<Entity> DenseEntities => _denseToEntity;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int PageOf(int id) => id / 1024;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int LocalOf(int id) => id % SparsePageSize;

    private void EnsurePage(int page)
    {
        while (_sparsePages.Count <= page)
        {
            var arr = new int[SparsePageSize];
            Array.Fill(arr, Tombstone);
            _sparsePages.Add(arr);
        }
    }

    private void SetDenseIndex(int id, int denseIndex)
    {
        var page = PageOf(id);
        var local = LocalOf(id);

        EnsurePage(page);
        _sparsePages[page][local] = denseIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetDenseIndex(int id)
    {
        var page = PageOf(id);
        var local = LocalOf(id);
        return page >= _sparsePages.Count
            ? Tombstone
            : _sparsePages[page][local];
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(Entity e) => GetDenseIndex(e.Id) != Tombstone;

    public ref T Set(Entity e, in T value)
    {
        var index = GetDenseIndex(e.Id);
        if (index != Tombstone)
        {
            _dense[index] = value;
            _denseToEntity[index] = e;
            return ref GetDenseRefByIndex(index);
        }

        var newIndex = _dense.Count;
        SetDenseIndex(e.Id, newIndex);

        _dense.Add(value);
        _denseToEntity.Add(e);

        return ref GetDenseRefByIndex(newIndex);
    }

    public bool TryGet(Entity e, out T value)
    {
        var index = GetDenseIndex(e.Id);
        if (index == Tombstone)
        {
            value = default!;
            return false;
        }

        value = _dense[index];
        return true;
    }

    public ref T GetRef(Entity e)
    {
        var index = GetDenseIndex(e.Id);
        if (index == Tombstone)
        {
            throw new InvalidOperationException($"Entity {e} not in SparseSet<{typeof(T).Name}>");
        }

        return ref GetDenseRefByIndex(index);
    }

    public Span<T> AsSpan() => CollectionsMarshal.AsSpan(_dense);

    public bool Delete(Entity e)
    {
        var deletedIndex = GetDenseIndex(e.Id);
        if (deletedIndex == Tombstone || _dense.Count == 0)
        {
            return false;
        }

        var lastIndex = _dense.Count - 1;
        var lastEntity = _denseToEntity[lastIndex];

        // Remap last entity -> deletedIndex
        SetDenseIndex(lastEntity.Id, deletedIndex);

        // Mark removed entity as tombstone
        SetDenseIndex(e.Id, Tombstone);

        if (deletedIndex != lastIndex)
        {
            _dense[deletedIndex] = _dense[lastIndex];
            _denseToEntity[deletedIndex] = lastEntity;
        }

        _dense.RemoveAt(lastIndex);
        _denseToEntity.RemoveAt(lastIndex);

        return true;
    }

    public void Clear()
    {
        _dense.Clear();
        _denseToEntity.Clear();
        _sparsePages.Clear();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetDenseRefByIndex(int index)
    {
        return ref CollectionsMarshal.AsSpan(_dense)[index];
    }
}