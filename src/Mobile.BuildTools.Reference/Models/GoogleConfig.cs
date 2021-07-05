using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class GoogleConfig
    {
        [JsonProperty("servicesJson", NullValueHandling = NullValueHandling.Ignore)]
        public string ServicesJson { get; set; }
        
        [JsonProperty("infoPlist", NullValueHandling = NullValueHandling.Ignore)]
        public string InfoPlist { get; set; }
    }
}
