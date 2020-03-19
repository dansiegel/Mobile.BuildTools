using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class WatermarkConfiguration
    {
        [JsonProperty("sourceFile")]
        public string SourceFile { get; set; }

        [JsonProperty("colors")]
        public IEnumerable<string> Colors { get; set; }

        [JsonProperty("position")]
        public WatermarkPosition? Position { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("textColor")]
        public string TextColor { get; set; }

        [JsonProperty("fontFamily")]
        public string FontFamily { get; set; }

        [JsonProperty("fontFile")]
        public string FontFile { get; set; }

        [JsonProperty("opacity")]
        public double? Opacity { get; set; }
    }
}
