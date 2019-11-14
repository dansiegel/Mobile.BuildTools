using System.Collections.Generic;

namespace Mobile.BuildTools.Models
{
    public class ImageResize
    {
        public IEnumerable<string> Directories { get; set; }

        public IDictionary<string, IEnumerable<string>> ConditionalDirectories { get; set; }

        public double? WatermarkOpacity { get; set; }
    }
}
