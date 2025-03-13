namespace Alstom.Spectrail.Server.Common.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RepositoryOperationAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}