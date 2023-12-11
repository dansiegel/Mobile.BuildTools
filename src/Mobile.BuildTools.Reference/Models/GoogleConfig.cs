using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models
{
    public class GoogleConfig
    {
        [JsonPropertyName("servicesJson")]
        public string ServicesJson { get; set; }
        
        [JsonPropertyName("infoPlist")]
        public string InfoPlist { get; set; }
    }
}
