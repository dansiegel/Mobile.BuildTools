using System.Collections.Generic;
using System.Linq;
using Mobile.BuildTools.Models.Settings;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Reference.Models.Settings
{
    public class SettingsConfig
    {
        public SettingsConfig()
        {
            Properties = new List<ValueConfig>();
        }

        [JsonProperty("delimiter")]
        public string Delimiter { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("className")]
        public string ClassName { get; set; }

        [JsonProperty("accessibility")]
        public Accessibility Accessibility { get; set; }

        [JsonProperty("rootNamespace")]
        public string RootNamespace { get; set; }

        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("properties")]
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
