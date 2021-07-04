using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class GoogleConfig
    {
        [JsonProperty("servicesJson", Required = Required.AllowNull)]
        public string ServicesJson { get; set; }
        
        [JsonProperty("infoPlist", Required = Required.AllowNull)]
        public string InfoPlist { get; set; }
    }
}
