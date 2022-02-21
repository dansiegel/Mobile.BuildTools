using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class AppleIconSetImage
    {
        [JsonPropertyName("scale")]
        public string Scale { get; set; }

        [JsonPropertyName("size")]
        public string Size { get; set; }

        [JsonPropertyName("idiom")]
        public string Idiom { get; set; }

        [JsonPropertyName("filename")]
        public string FileName { get; set; }
    }
}
