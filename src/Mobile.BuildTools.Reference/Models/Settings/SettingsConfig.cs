using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mobile.BuildTools.Models.Secrets;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Models.Settings
{
    public class SettingsConfig
    {
        [DefaultValue(";")]
        [Description("The delimiter used for arrays. By default this will use a semi-colon.")]
        [JsonPropertyName("delimiter")]
        public string Delimiter { get; set; } = ";";

        [DefaultValue("BuildTools_")]
        [Description("The prefix the Mobile.BuildTools should use to look for variables. Note if a variable exists with the exact name it will be used if one does not exist with the prefix.")]
        [JsonPropertyName("prefix")]
        public string Prefix { get; set; } = "BuildTools_";

        [DefaultValue("AppSettings")]
        [Description("The name of the generated class.")]
        [JsonPropertyName("className")]
        public string ClassName { get; set; } = "AppSettings";

        [DefaultValue(Accessibility.Internal)]
        [Description("The default visibility of the generated class, either 'public' or 'internal'.")]
        [JsonPropertyName("accessibility")]
        public Accessibility Accessibility { get; set; } = Accessibility.Internal;

        [Description("If using a Shared project as is typically the case with an Uno Platform app, be sure to specify the Root Namespace to use as this will change otherwise based on which platform target you are compiling.")]
        [JsonPropertyName("rootNamespace")]
        public string RootNamespace { get; set; }

        [DefaultValue("Helpers")]
        [Description("The partial relative namespace to generate. By default this will be the Helpers namespace, you may set it to the root namespace by providing a period '.' for the value.")]
        [JsonPropertyName("namespace")]
        public string Namespace { get; set; } = "Helpers";

        [Description("The properties that should be generated in the generated AppSettings class.")]
        [JsonPropertyName("properties")]
        public List<ValueConfig> Properties { get; set; } = [];

        [JsonConverter(typeof(PlatformArrayJsonConverter))]
        public Platform[] RequiredPlatforms { get; set; } = [];

        [JsonIgnore]
        public string FullyQualifiedClassName => $"{RootNamespace}.{Namespace}.{ClassName}";

        public bool ContainsKey(string key) => Properties != null && Properties.Any(x => x.Name == key);

        public bool HasKey(string key, out ValueConfig config)
        {
            config = this[key];
            return config != null;
        }

        public ValueConfig this[string key] => Properties?.FirstOrDefault(x => x.Name == key);
    }
}
