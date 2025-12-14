namespace Engine.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class UpdateAfterAttribute(Type other) : Attribute
{
    public Type Other { get; } = other;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class UpdateBeforeAttribute(Type other) : Attribute
{
    public Type Other { get; } = other;
}