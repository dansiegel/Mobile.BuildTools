using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models
{
    public class EnvironmentSettings
    {
        [JsonPropertyName("enableFuzzyMatching")]
        public bool EnableFuzzyMatching { get; set; }

        [JsonPropertyName("defaults")]
        public Dictionary<string, string> Defaults { get; set; } = [];

        [JsonPropertyName("configuration")]
        public Dictionary<string, Dictionary<string, string>> Configuration { get; set; } = [];
    }
}
