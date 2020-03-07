using System.ComponentModel;
using System.IO;
using Mobile.BuildTools.Tasks.Utils;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class ResourceDefinition : PlatformConfiguration
    {
        [JsonIgnore]
        public string InputFilePath { get; set; }

        [JsonProperty("watermarkFile")]
        public string WatermarkFile { get; set; }

        [JsonProperty("android")]
        public AndroidConfiguration Android { get; set; }

        [JsonProperty("apple")]
        public PlatformConfiguration Apple { get; set; }

        public string GetBackgroundColor(Platform platform)
        {
            switch (platform)
            {
                case Platform.Android:
                    return string.IsNullOrEmpty(Android?.BackgroundColor) ? BackgroundColor : Android.BackgroundColor;
                case Platform.iOS:
                case Platform.macOS:
                    return string.IsNullOrEmpty(Apple?.BackgroundColor) ? BackgroundColor : Apple.BackgroundColor;
                default:
                    return BackgroundColor;
            }
        }

        public string GetName(Platform platform)
        {
            switch(platform)
            {
                case Platform.Android:
                    return string.IsNullOrEmpty(Android?.Name) ? Name : Android.Name;
                case Platform.iOS:
                case Platform.macOS:
                    return string.IsNullOrEmpty(Apple?.Name) ? Name : Apple.Name;
                default:
                    return Name;
            }
        }

        public (string Name, bool Ignore, double Scale) GetConfiguration(Platform platform)
        {
            var name = GetName(platform);
            var ignore = ShouldIgnore(platform);
            var scale = Scale;

            // get platform scale
            switch (platform)
            {
                case Platform.Android:
                    scale = Android != null && Android.Scale > 0 ? Android.Scale : Scale;
                    break;
                case Platform.iOS:
                case Platform.macOS:
                case Platform.TVOS:
                    scale = Apple != null && Apple.Scale > 0 ? Apple.Scale : Scale;
                    break;
            }

            if(string.IsNullOrEmpty(name))
            {
                name = Path.GetFileNameWithoutExtension(InputFilePath);
            }

            return (name, ignore, scale);
        }

        public bool ShouldIgnore(Platform platform)
        {
            switch (platform)
            {
                case Platform.Android:
                    return Ignore || (Android?.Ignore ?? false);
                case Platform.iOS:
                case Platform.macOS:
                    return Ignore || (Apple?.Ignore ?? false);
            }

            return Ignore;
        }
    }

    public class AndroidConfiguration : PlatformConfiguration
    {
        [DefaultValue(AndroidResource.Drawable)]
        [JsonProperty("resourceType")]
#if SCHEMAGENERATOR
        [System.ComponentModel.DataAnnotations.EnumDataType(typeof(AndroidResource))]
        public string ResourceType { get; set; }
#else
        public AndroidResource ResourceType { get; set; }
#endif
    }
}
