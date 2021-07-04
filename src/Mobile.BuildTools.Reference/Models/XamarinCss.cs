using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class XamarinCss : ToolItem
    {
        [DefaultValue(false)]
        [JsonProperty("minify", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Minify { get; set; }

        [DefaultValue(false)]
        [JsonProperty("bundleScss", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool BundleScss { get; set; }
    }
}
