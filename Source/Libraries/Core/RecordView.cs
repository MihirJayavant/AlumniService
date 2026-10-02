namespace Core;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RecordViewAttribute(Type sourceType, params string[] exclude) : Attribute
{
    public Type SourceType { get; } = sourceType;
    public string[] Exclude { get; } = exclude;
}


