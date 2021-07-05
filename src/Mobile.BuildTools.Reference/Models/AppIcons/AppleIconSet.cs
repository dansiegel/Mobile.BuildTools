using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class AppleIconSet
    {
        [JsonProperty("images", NullValueHandling = NullValueHandling.Include)]
        public List<AppleIconSetImage> Images { get; set; }
    }
}
