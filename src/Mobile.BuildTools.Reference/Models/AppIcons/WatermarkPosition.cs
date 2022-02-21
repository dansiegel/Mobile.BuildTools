using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models.AppIcons
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
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
