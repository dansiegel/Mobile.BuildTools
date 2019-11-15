using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class XamarinCss : ToolItem
    {
        [DefaultValue(false)]
        [JsonProperty("minify")]
        public bool Minify { get; set; }

        [DefaultValue(false)]
        [JsonProperty("bundleScss")]
        public bool BundleScss { get; set; }
    }
}
