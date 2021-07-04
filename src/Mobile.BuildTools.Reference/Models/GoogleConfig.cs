using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class GoogleConfig
    {
        [JsonProperty("servicesJson", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string ServicesJson { get; set; }
        
        [JsonProperty("infoPlist", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string InfoPlist { get; set; }
    }
}
