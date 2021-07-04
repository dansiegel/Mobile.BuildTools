using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.Settings
{
    public class SettingsConfig
    {
        public SettingsConfig()
        {
            Properties = new List<ValueConfig>();
        }

        [DefaultValue(";")]
        [Description("The delimiter used for arrays. By default this will use a semi-colon.")]
        [JsonProperty("delimiter", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Delimiter { get; set; }

        [DefaultValue("BuildTools_")]
        [Description("The prefix the Mobile.BuildTools should use to look for variables. Note if a variable exists with the exact name it will be used if one does not exist with the prefix.")]
        [JsonProperty("prefix", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Prefix { get; set; }

        [DefaultValue("AppSettings")]
        [Description("The name of the generated class.")]
        [JsonProperty("className", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string ClassName { get; set; }

        [DefaultValue(Accessibility.Internal)]
        [Description("The default visibility of the generated class, either 'public' or 'internal'.")]
        [JsonProperty("accessibility", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Accessibility Accessibility { get; set; }

        [Description("If using a Shared project as is typically the case with an Uno Platform app, be sure to specify the Root Namespace to use as this will change otherwise based on which platform target you are compiling.")]
        [JsonProperty("rootNamespace", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string RootNamespace { get; set; }

        [DefaultValue("Helpers")]
        [Description("The partial relative namespace to generate. By default this will be the Helpers namespace, you may set it to the root namespace by providing a period '.' for the value.")]
        [JsonProperty("namespace", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }

        [Description("The properties that should be generated in the generated AppSettings class.")]
        [JsonProperty("properties", Required = Required.Always)]
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
