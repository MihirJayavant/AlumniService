using System;

namespace Domain
{
[AttributeUsage(AttributeTargets.Class)]
public sealed class RecordView: Attribute
{
    public RecordView(Type sourceType, params string[] exclude)
    {
        SourceType = sourceType;
        Exclude = exclude;
    }
    public Type SourceType { get; }
    public string[] Exclude { get; }
}

}

