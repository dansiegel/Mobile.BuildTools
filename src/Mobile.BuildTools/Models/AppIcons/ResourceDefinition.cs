using System;
using System.Text;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class ResourceDefinition : PlatformConfiguration
    {
        public string InputFilePath { get; set; }

        public string WatermarkFile { get; set; }

        public AndroidConfiguration Android { get; set; }

        public PlatformConfiguration Apple { get; set; }
    }

    public class AndroidConfiguration
    {
        public AndroidResource ResourceType { get; set; }
    }
}
