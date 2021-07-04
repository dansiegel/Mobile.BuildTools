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

        [JsonProperty("directories", Required = Required.AllowNull)]
        public List<string> Directories { get; set; }

        [JsonProperty("conditionalDirectories", Required = Required.AllowNull)]
        public Dictionary<string, IEnumerable<string>> ConditionalDirectories { get; set; }
    }
}
