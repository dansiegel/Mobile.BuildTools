using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class EnvironmentSettings
    {
        public EnvironmentSettings()
        {
            Defaults = new Dictionary<string, string>();
            Configuration = new Dictionary<string, Dictionary<string, string>>();
        }

        [JsonProperty("defaults")]
        public Dictionary<string, string> Defaults { get; set; }

        [JsonProperty("configuration")]
        public Dictionary<string, Dictionary<string, string>> Configuration { get; set; }
    }
}
