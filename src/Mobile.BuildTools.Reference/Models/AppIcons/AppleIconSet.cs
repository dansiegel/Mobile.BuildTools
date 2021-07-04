using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class AppleIconSet
    {
        [JsonProperty("images", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
        public List<AppleIconSetImage> Images { get; set; }
    }
}
