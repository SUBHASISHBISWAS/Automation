namespace SpectrailTestFramework.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class MapsToPageAttribute : Attribute
{
    public MapsToPageAttribute(Type pageType)
    {
        PageType = pageType ?? throw new ArgumentNullException(nameof(pageType));
    }

    public Type PageType { get; }
}