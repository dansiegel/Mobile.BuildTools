using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class XamarinCss : ToolItem
    {
        [JsonProperty("minify")]
        public bool Minify { get; set; }

        [JsonProperty("bundleScss")]
        public bool BundleScss { get; set; }
    }
}
