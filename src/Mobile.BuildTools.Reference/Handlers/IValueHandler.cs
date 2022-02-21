using System;
using System.Text.Json;

namespace Mobile.BuildTools.Handlers
{
    public interface IValueHandler
    {
        string Format(string rawValue, bool safeOutput);
    }

    internal static class IValueHandlerExtensions
    {
        public static string Format(this IValueHandler handler, JsonProperty value, bool safeOutput) =>
            handler.Format(value.GetPropertyValueAsString(), safeOutput);

        internal static string GetPropertyValueAsString(this JsonProperty setting) =>
            setting.Value.ValueKind switch
            {
                JsonValueKind.String => setting.Value.GetString(),
                JsonValueKind.Number => setting.Value.GetDouble().ToString(),
                JsonValueKind.Null => string.Empty,
                JsonValueKind.False => bool.FalseString,
                JsonValueKind.True => bool.TrueString,
                _ => throw new NotSupportedException($"The Json Value type of {setting.Value.ValueKind} is not supported for AppSetting properties")
            };
    }
}
