using System;
using System.Linq;
using Microsoft.Build.Framework;
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
            var images = Images.Select(x => x.ToOutputImage());

            if (!images.Any()) return;

            var generator = new ImageResizeGenerator(this)
            {
                OutputImages = images,
            };

            generator.Execute();
            GeneratedImages = generator.Outputs.Select(x => x.ToTaskItem()).ToArray();
        }
    }
}
