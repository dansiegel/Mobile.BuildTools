using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class BaseImageConfiguration : IImageResource
    {
        [JsonProperty("ignore")]
        public bool Ignore { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("scale")]
        public double Scale { get; set; }

        [JsonProperty("backgroundColor")]
        public string BackgroundColor { get; set; }

        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonProperty("height")]
        public int? Height { get; set; }

        [JsonProperty("padFactor")]
        public double? PaddingFactor { get; set; }

        [JsonProperty("padColor")]
        public string PaddingColor { get; set; }

        [JsonProperty("watermark")]
        public WatermarkConfiguration Watermark { get; set; }

        [JsonProperty("resourceType")]
#if SCHEMAGENERATOR
        [System.ComponentModel.DataAnnotations.EnumDataType(typeof(AndroidResource))]
        public string ResourceType { get; set; }
#else
        [DefaultValue(AndroidResource.Drawable)]
        public AndroidResource ResourceType { get; set; }
#endif
        [JsonIgnore]
        public string SourceFile { get; set; }
    }
}
