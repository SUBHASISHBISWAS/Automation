using System;

namespace SpectrailTestFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class MapsToPageAttribute : Attribute
    {
        public Type PageType { get; }

        public MapsToPageAttribute(Type pageType)
        {
            PageType = pageType ?? throw new ArgumentNullException(nameof(pageType));
        }
    }
}