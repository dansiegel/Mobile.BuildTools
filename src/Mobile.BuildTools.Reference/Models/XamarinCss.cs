using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class XamarinCss : ToolItem
    {
        [DefaultValue(false)]
        [JsonProperty("minify", NullValueHandling = NullValueHandling.Ignore)]
        public bool Minify { get; set; }

        [DefaultValue(false)]
        [JsonProperty("bundleScss", NullValueHandling = NullValueHandling.Ignore)]
        public bool BundleScss { get; set; }
    }
}
