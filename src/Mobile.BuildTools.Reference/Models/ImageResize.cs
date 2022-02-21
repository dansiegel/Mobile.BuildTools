using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models
{
    public class ImageResize : ToolItem
    {
        public ImageResize()
        {
            Directories = new List<string>();
            ConditionalDirectories = new Dictionary<string, IEnumerable<string>>();
        }

        [JsonPropertyName("directories")]
        public List<string> Directories { get; set; }

        [JsonPropertyName("conditionalDirectories")]
        public Dictionary<string, IEnumerable<string>> ConditionalDirectories { get; set; }
    }
}
