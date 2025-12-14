namespace Engine;

public readonly struct Entity(int id)
{
    public readonly int Id = id;

    public override string ToString() => $"Entity({Id})";
}