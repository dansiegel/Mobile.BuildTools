using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.Secrets
{
    public class SecretsConfig
    {
        public SecretsConfig()
        {
            Properties = new List<ValueConfig>();
        }

        [JsonProperty("disable")]
        public bool Disable { get; set; }

        [JsonProperty("delimiter")]
        public string Delimiter { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("className")]
        public string ClassName { get; set; }

#if SCHEMAGENERATOR
        [System.ComponentModel.DefaultValue("Internal")]
        [System.ComponentModel.DataAnnotations.EnumDataType(typeof(Accessibility))]
        public string Accessibility { get; set; }
#else
        [JsonProperty("accessibility")]
        public Accessibility Accessibility { get; set; }
#endif

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
