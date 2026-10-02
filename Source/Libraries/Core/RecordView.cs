namespace Core;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RecordView(Type sourceType, params string[] exclude) : Attribute
{
    public Type SourceType { get; } = sourceType;
    public string[] Exclude { get; } = exclude;
}



