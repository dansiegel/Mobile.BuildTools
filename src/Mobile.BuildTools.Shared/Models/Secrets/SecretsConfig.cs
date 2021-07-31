using System.Collections.Generic;
using System.Linq;
using Mobile.BuildTools.Models.Settings;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.Secrets
{
    public class SecretsConfig
    {
        public SecretsConfig()
        {
            Properties = new List<ValueConfig>();
        }

        [JsonProperty("disable", NullValueHandling = NullValueHandling.Ignore)]
        public bool Disable { get; set; }

        [JsonProperty("delimiter", NullValueHandling = NullValueHandling.Ignore)]
        public string Delimiter { get; set; }

        [JsonProperty("prefix", NullValueHandling = NullValueHandling.Ignore)]
        public string Prefix { get; set; }

        [JsonProperty("className", NullValueHandling = NullValueHandling.Ignore)]
        public string ClassName { get; set; }

        [JsonProperty("accessibility", NullValueHandling = NullValueHandling.Ignore)]
        public Accessibility Accessibility { get; set; }

        [JsonProperty("rootNamespace", NullValueHandling = NullValueHandling.Ignore)]
        public string RootNamespace { get; set; }

        [JsonProperty("namespace", NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
        public List<ValueConfig> Properties { get; set; }

        public bool ContainsKey(string key) => Properties != null && Properties.Any(x => x.Name == key);

        public bool HasKey(string key, out ValueConfig config)
        {
            config = this[key];
            return config != null;
        }

        public ValueConfig this[string key] => Properties?.FirstOrDefault(x => x.Name == key);
    }
}
