using Newtonsoft.Json.Schema.Generation;

namespace Mobile.BuildTools.Models
{
    [JSchemaGenerationProvider(typeof(StringEnumGenerationProvider))]
    public enum VersionBehavior
    {
        Off,
        PreferBuildNumber,
        Timestamp
    }
}
