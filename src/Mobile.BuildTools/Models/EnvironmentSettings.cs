using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class EnvironmentSettings
    {
        [JsonProperty("defaults")]
        public Dictionary<string, string> Defaults { get; set; }

        [JsonProperty("configuration")]
        public Dictionary<string, Dictionary<string, string>> Configuration { get; set; }
    }
}
