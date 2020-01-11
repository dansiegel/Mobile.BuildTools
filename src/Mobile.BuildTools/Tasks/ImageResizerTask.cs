using System;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Models.AppIcons;

namespace Mobile.BuildTools.Tasks
{
    public class ImageResizerTask : BuildToolsTaskBase
    {
        [Required]
        public ITaskItem[] Images { get; set; }

        [Output]
        public ITaskItem[] GeneratedImages { get; set; }

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            GeneratedImages = Array.Empty<ITaskItem>();
            var images = Images.Select(x => OutputImage.FromTaskItem(x));

            if (!images.Any()) return;

            var generator = new ImageResizeGenerator(this)
            {
                IntermediateOutputDirectory = IntermediateOutputPath,
                OutputImages = images,
                WatermarkOpacity = config.Configuration.Images.WatermarkOpacity
            };

            generator.Execute();
            GeneratedImages = Images;
        }
    }
}
