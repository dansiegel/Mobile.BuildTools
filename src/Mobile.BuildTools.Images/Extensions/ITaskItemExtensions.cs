using System;
using Microsoft.Build.Framework;

namespace Mobile.BuildTools.Extensions
{
    public static class ITaskItemExtensions
    {
        public static string GetMetadata(this ITaskItem item, string metadataName, string defaultValue)
        {
            var value = item?.GetMetadata(metadataName);
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return value;
        }

        public static TEnum GetEnumFromMetadata<TEnum>(this ITaskItem item, string metadataName, TEnum defaultValue)
             where TEnum : struct
        {
            var value = item?.GetMetadata(metadataName);

            if (!string.IsNullOrEmpty(value) && Enum.TryParse<TEnum>(value, out var output))
                return output;

            return defaultValue;
        }
    }
}
