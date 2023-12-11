using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models.AppIcons
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PlatformResourceType
    {
        Default,
        Drawable,
        Mipmap,
        AllSquareTiles,
        SquareTile,
        SmallTile,
        SplashScreen,
        WideTile
    }
}
