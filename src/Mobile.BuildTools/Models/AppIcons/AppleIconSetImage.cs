using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class AppleIconSetImage
    {
        [JsonProperty("scale")]
        public string Scale { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("idiom")]
        public string Idiom { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }
    }
}
