using Newtonsoft.Json.Schema.Generation;

namespace Mobile.BuildTools.Models.AppIcons
{
    [JSchemaGenerationProvider(typeof(StringEnumGenerationProvider))]
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
