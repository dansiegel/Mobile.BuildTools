using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.Secrets
{
    public class SecretsConfig : Dictionary<string, ValueConfig>
    {
        [JsonProperty("delimiter")]
        public string Delimiter { get; set; }
    }
}
