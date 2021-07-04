using Newtonsoft.Json;
using System.ComponentModel;

namespace Mobile.BuildTools.Models
{
    public abstract class ToolItem
    {
        [Description("Disables this Mobile.Build.Tools Task")]
        [DefaultValue(false)]
        [JsonProperty("disable", Required = Required.AllowNull)]
        public bool Disable { get; set; }
    }
}
