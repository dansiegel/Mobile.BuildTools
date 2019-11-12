using System.Collections.Generic;

namespace Mobile.BuildTools.Models
{
    public class ImageResize
    {
        public IList<string> Directories { get; set; }

        public IDictionary<string, string> ConditionalDirectories { get; set; } 
    }
}
