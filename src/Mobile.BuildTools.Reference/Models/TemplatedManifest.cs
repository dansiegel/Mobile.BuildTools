using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class TemplatedManifest : ToolItem
    {
        [DefaultValue("$$")]
        [JsonProperty("token", Required = Required.AllowNull)]
        public string Token { get; set; }

        [DefaultValue("Manifest_")]
        [JsonProperty("variablePrefix", Required = Required.AllowNull)]
        public string VariablePrefix { get; set; }

        [DefaultValue(false)]
        [JsonProperty("missingTokensAsErrors", Required = Required.AllowNull)]
        public bool MissingTokensAsErrors { get; set; }
    }
}
