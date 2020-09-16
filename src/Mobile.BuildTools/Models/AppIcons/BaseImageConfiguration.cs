using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class BaseImageConfiguration : IImageResource
    {
        [JsonProperty("ignore", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Ignore { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("scale", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double Scale { get; set; }

        [JsonProperty("backgroundColor", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string BackgroundColor { get; set; }

        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Width { get; set; }

        [JsonProperty("height", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Height { get; set; }

        [JsonProperty("padFactor", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double? PaddingFactor { get; set; }

        [JsonProperty("padColor", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PaddingColor { get; set; }

        [JsonProperty("watermark", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public WatermarkConfiguration Watermark { get; set; }

        [JsonProperty("resourceType", DefaultValueHandling = DefaultValueHandling.Ignore)]
#if SCHEMAGENERATOR
        [System.ComponentModel.DataAnnotations.EnumDataType(typeof(AndroidResource))]
        public string ResourceType { get; set; }
#else
        public PlatformResourceType ResourceType { get; set; }
#endif
        [JsonIgnore]
        public string SourceFile { get; set; }
    }
}
