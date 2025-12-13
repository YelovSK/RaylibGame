namespace Engine;

public readonly struct Entity(uint id)
{
    public readonly uint Id = id;

    public override string ToString() => $"Entity({Id})";
}