using Newtonsoft.Json.Linq;

namespace Mobile.BuildTools.Handlers
{
    internal interface IValueHandler
    {
        string Format(string rawValue, bool safeOutput);
    }

    internal static class IValueHandlerExtensions
    {
        public static string Format(this IValueHandler handler, JToken value, bool safeOutput) =>
            handler.Format(value.ToString(), safeOutput);
    }
}
