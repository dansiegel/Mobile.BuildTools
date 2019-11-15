using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class ImageResize : ToolItem
    {
        [JsonProperty("directories")]
        public List<string> Directories { get; set; }

        [JsonProperty("conditionalDirectories")]
        public Dictionary<string, IEnumerable<string>> ConditionalDirectories { get; set; }

        [JsonProperty("watermarkOpacity")]
        public double? WatermarkOpacity { get; set; }
    }
}
