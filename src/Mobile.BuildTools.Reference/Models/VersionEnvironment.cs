using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VersionEnvironment
    {
        All,
        BuildHost,
        Local
    }
}
