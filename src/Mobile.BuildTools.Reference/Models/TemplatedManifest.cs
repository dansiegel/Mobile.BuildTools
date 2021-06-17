using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class TemplatedManifest : ToolItem
    {
        [DefaultValue("$$")]
        [JsonProperty("token")]
        public string Token { get; set; }

        [DefaultValue("Manifest_")]
        [JsonProperty("variablePrefix")]
        public string VariablePrefix { get; set; }

        [DefaultValue(false)]
        [JsonProperty("missingTokensAsErrors")]
        public bool MissingTokensAsErrors { get; set; }
    }
}
