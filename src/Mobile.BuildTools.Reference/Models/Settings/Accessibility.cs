using Newtonsoft.Json.Schema.Generation;

namespace Mobile.BuildTools.Models.Settings
{
    [JSchemaGenerationProvider(typeof(StringEnumGenerationProvider))]
    public enum Accessibility
    {
        Internal,
        Public
    }
}
