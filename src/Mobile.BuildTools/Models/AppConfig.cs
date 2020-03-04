using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class AppConfig : ToolItem
    {
        [DefaultValue(false)]
        [JsonProperty("includeAllConfigs")]
        public bool IncludeAllConfigs { get; set; }
    }
}
