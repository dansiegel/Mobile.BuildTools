using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class TemplatedManifest : ToolItem
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("variablePrefix")]
        public string VariablePrefix { get; set; }

        [JsonProperty("missingTokensAsErrors")]
        public bool MissingTokensAsErrors { get; set; }
    }
}
