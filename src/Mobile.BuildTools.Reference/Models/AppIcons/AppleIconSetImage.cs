using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class AppleIconSetImage
    {
        [JsonProperty("scale", Required = Required.AllowNull)]
        public string Scale { get; set; }

        [JsonProperty("size", Required = Required.AllowNull)]
        public string Size { get; set; }

        [JsonProperty("idiom", Required = Required.AllowNull)]
        public string Idiom { get; set; }

        [JsonProperty("filename", Required = Required.AllowNull)]
        public string FileName { get; set; }
    }
}
