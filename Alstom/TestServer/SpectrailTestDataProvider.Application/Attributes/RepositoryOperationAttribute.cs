namespace SpectrailTestDataProvider.Application.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RepositoryOperationAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}