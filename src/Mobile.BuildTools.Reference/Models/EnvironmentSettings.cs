using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models
{
    public class EnvironmentSettings
    {
        public EnvironmentSettings()
        {
            Defaults = new Dictionary<string, string>();
            Configuration = new Dictionary<string, Dictionary<string, string>>();
        }

        [JsonPropertyName("defaults")]
        public Dictionary<string, string> Defaults { get; set; }

        [JsonPropertyName("configuration")]
        public Dictionary<string, Dictionary<string, string>> Configuration { get; set; }
    }
}
