using System.Linq;
using System.Reflection;

namespace Mobile.BuildTools.Models.Secrets
{
    public static class PropertyTypeExtensions
    {
        public static PropertyTypeMappingAttribute GetPropertyTypeMapping(this PropertyType propertyType)
        {
            var type = typeof(PropertyType);
            var memberInfoArray = type.GetMember(propertyType.ToString());
            var member = memberInfoArray.FirstOrDefault(x => x.DeclaringType == typeof(PropertyType));
            return member.GetCustomAttribute<PropertyTypeMappingAttribute>();
        }
    }
}
