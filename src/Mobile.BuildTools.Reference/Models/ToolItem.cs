using Newtonsoft.Json;
using System.ComponentModel;

namespace Mobile.BuildTools.Models
{
    public abstract class ToolItem
    {
        [Description("Disables this Mobile.BuildTools Task")]
        [DefaultValue(false)]
        [JsonProperty("disable", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool? Disable { get; set; }
    }
}
