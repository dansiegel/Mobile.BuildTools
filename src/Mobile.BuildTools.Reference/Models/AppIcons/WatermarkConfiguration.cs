using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class WatermarkConfiguration
    {
        [Description("Specifies the watermark source file name.")]
        [JsonPropertyName("sourceFile")]
        public string SourceFile { get; set; }

        [Description("Specifies the color or colors (for gradients) that should be used for the automatically generated Banner style watermark.")]
        [JsonPropertyName("colors")]
        public IEnumerable<string> Colors { get; set; }

        [Description("Specifies the watermark position.")]
        [JsonPropertyName("position")]
        public WatermarkPosition? Position { get; set; }

        [Description("Specifies the text for the watermark. For example you may wish to have a banner such as 'Dev' or 'Stage' to signify which environment the app was built for. Other scenarios could include 'Free', 'Pro', 'Lite', etc.")]
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [Description("The hex or name of the color to use for the text.")]
        [JsonPropertyName("textColor")]
        public string TextColor { get; set; }

        [Description("The Font Family name if you want to use a custom font.")]
        [JsonPropertyName("fontFamily")]
        public string FontFamily { get; set; }

        [Description("Have a custom font you want to use, you can provide the file path to the Font File.")]
        [JsonPropertyName("fontFile")]
        public string FontFile { get; set; }

        [Description("The opacity to use for the Watermark or Banner.")]
        [JsonPropertyName("opacity")]
        public double? Opacity { get; set; }
    }
}
