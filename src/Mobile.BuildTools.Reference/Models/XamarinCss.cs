using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models
{
    public class XamarinCss : ToolItem
    {
        [DefaultValue(false)]
        [JsonPropertyName("minify")]
        public bool Minify { get; set; }

        [DefaultValue(false)]
        [JsonPropertyName("bundleScss")]
        public bool BundleScss { get; set; }
    }
}
