using Newtonsoft.Json.Schema.Generation;

namespace Mobile.BuildTools.Models.AppIcons
{
    [JSchemaGenerationProvider(typeof(StringEnumGenerationProvider))]
    public enum WatermarkPosition
    {
        Top,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    };
}
