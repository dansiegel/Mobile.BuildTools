using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class ImageResize : ToolItem
    {
        public ImageResize()
        {
            Directories = new List<string>();
            ConditionalDirectories = new Dictionary<string, IEnumerable<string>>();
        }

        [JsonProperty("directories")]
        public List<string> Directories { get; set; }

        [JsonProperty("conditionalDirectories")]
        public Dictionary<string, IEnumerable<string>> ConditionalDirectories { get; set; }

        [JsonProperty("watermarkOpacity")]
        public double? WatermarkOpacity { get; set; }
    }
}
