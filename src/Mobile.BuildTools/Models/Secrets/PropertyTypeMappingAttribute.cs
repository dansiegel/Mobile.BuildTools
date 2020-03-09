using System;
using Mobile.BuildTools.Handlers;

namespace Mobile.BuildTools.Models.Secrets
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class PropertyTypeMappingAttribute : Attribute
    {
        public PropertyTypeMappingAttribute(Type type)
            : this(type, typeof(DefaultValueHandler))
        {
        }

        public PropertyTypeMappingAttribute(Type type, string format)
        {
            Type = type;
            Handler = new DefaultValueHandler(format);
        }

        public PropertyTypeMappingAttribute(Type type, Type handlerType)
        {
            Type = type;
            Handler = Activator.CreateInstance(handlerType) as IValueHandler;
        }

        public Type Type { get; }

        //public string Format { get; }

        public IValueHandler Handler { get; }
    }
}
