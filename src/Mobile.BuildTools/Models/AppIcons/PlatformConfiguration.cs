using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class PlatformConfiguration
    {
        [JsonProperty("ignore")]
        public bool Ignore { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("scale")]
        public double Scale { get; set; }
    }
}
