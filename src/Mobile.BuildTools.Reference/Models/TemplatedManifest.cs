using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class TemplatedManifest : ToolItem
    {
        [Description("The Regex escaped value of the Token. '$$' is used by default to look for token matching the pattern $TokenName$.")]
        [DefaultValue("$$")]
        [JsonProperty("token", NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; }

        [DefaultValue("Manifest_")]
        [JsonProperty("variablePrefix", NullValueHandling = NullValueHandling.Ignore)]
        public string VariablePrefix { get; set; }

        [Description("If set to true, this will generate a build time error if a token is found which does not have a value in the environment or secrets.json.")]
        [DefaultValue(false)]
        [JsonProperty("missingTokensAsErrors", NullValueHandling = NullValueHandling.Ignore)]
        public bool MissingTokensAsErrors { get; set; }
    }
}
