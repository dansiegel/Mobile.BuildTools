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

        [System.ComponentModel.Description("Minifies selected Lottie JSON assets. Enabled by default. Set false to preserve selected animation bytes while retaining conditional asset selection.")]
        [JsonPropertyName("optimizeLottie")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [System.ComponentModel.DefaultValue(true)]
        public bool OptimizeLottie { get; set; } = true;

        [JsonPropertyName("directories")]
        public List<string> Directories { get; set; }

        [JsonPropertyName("conditionalDirectories")]
        public Dictionary<string, IEnumerable<string>> ConditionalDirectories { get; set; }
    }
}

