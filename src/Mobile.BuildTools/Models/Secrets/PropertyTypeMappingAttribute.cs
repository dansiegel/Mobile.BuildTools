using System;

namespace Mobile.BuildTools.Models.Secrets
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class PropertyTypeMappingAttribute : Attribute
    {
        public PropertyTypeMappingAttribute(Type type)
            : this(type, "{0}")
        {
        }

        public PropertyTypeMappingAttribute(Type type, string format)
        {
            Type = type;
            Format = format;
        }

        public Type Type { get; }

        public string Format { get; }
    }
}
