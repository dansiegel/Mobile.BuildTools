using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models.Settings
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Accessibility
    {
        Internal,
        Public
    }
}
