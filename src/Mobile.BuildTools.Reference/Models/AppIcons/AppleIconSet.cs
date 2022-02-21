using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class AppleIconSet
    {
        [JsonPropertyName("images")]
        public List<AppleIconSetImage> Images { get; set; }
    }
}
