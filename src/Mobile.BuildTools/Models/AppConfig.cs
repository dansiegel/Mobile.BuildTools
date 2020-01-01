using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class AppConfig : ToolItem
    {
        [DefaultValue(true)]
        [JsonProperty("includeAllConfigs")]
        public bool IncludeAllConfigs { get; set; } = true;
    }
}
