using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models
{
    public abstract class ToolItem
    {
        [Description("Disables this Mobile.BuildTools Task")]
        [DefaultValue(false)]
        [JsonPropertyName("disable")]
        public bool? Disable { get; set; }
    }
}
