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

        [JsonProperty("disable", Required = Required.AllowNull)]
        public bool Disable { get; set; }

        [JsonProperty("delimiter", Required = Required.AllowNull)]
        public string Delimiter { get; set; }

        [JsonProperty("prefix", Required = Required.AllowNull)]
        public string Prefix { get; set; }

        [JsonProperty("className", Required = Required.AllowNull)]
        public string ClassName { get; set; }

        [JsonProperty("accessibility", Required = Required.AllowNull)]
        public Accessibility Accessibility { get; set; }

        [JsonProperty("rootNamespace", Required = Required.AllowNull)]
        public string RootNamespace { get; set; }

        [JsonProperty("namespace", Required = Required.AllowNull)]
        public string Namespace { get; set; }

        [JsonProperty("properties", Required = Required.AllowNull)]
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
