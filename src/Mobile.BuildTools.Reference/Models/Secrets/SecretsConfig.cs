using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Mobile.BuildTools.Models.Settings;

namespace Mobile.BuildTools.Models.Secrets
{
    public class SecretsConfig
    {
        public SecretsConfig()
        {
            Properties = new List<ValueConfig>();
        }

        [JsonPropertyName("disable")]
        public bool Disable { get; set; }

        [JsonPropertyName("delimiter")]
        public string Delimiter { get; set; }

        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }

        [JsonPropertyName("className")]
        public string ClassName { get; set; }

        [JsonPropertyName("accessibility")]
        public Accessibility Accessibility { get; set; }

        [JsonPropertyName("rootNamespace")]
        public string RootNamespace { get; set; }

        [JsonPropertyName("namespace")]
        public string Namespace { get; set; }

        [JsonPropertyName("properties")]
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
