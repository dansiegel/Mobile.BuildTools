using Newtonsoft.Json.Schema.Generation;

namespace Mobile.BuildTools.Models
{
    [JSchemaGenerationProvider(typeof(StringEnumGenerationProvider))]
    public enum VersionEnvironment
    {
        All,
        BuildHost,
        Local
    }
}
