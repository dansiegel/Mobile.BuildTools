using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models
{
    public class TemplatedManifest : ToolItem
    {
        [Description("The Regex escaped value of the Token. '$$' is used by default to look for token matching the pattern $TokenName$.")]
        [DefaultValue("$$")]
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [DefaultValue("Manifest_")]
        [JsonPropertyName("variablePrefix")]
        public string VariablePrefix { get; set; }

        [Description("If set to true, this will generate a build time error if a token is found which does not have a value in the environment or appsettings.json.")]
        [DefaultValue(false)]
        [JsonPropertyName("missingTokensAsErrors")]
        public bool MissingTokensAsErrors { get; set; }
    }
}
