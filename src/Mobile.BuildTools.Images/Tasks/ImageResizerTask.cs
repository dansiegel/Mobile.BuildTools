using System;
using System.Collections.Generic;
using System.IO;
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

        public ITaskItem[] SourceFiles { get; set; } = Array.Empty<ITaskItem>();

        [Output]
        public ITaskItem[] GeneratedImages { get; set; } = Array.Empty<ITaskItem>();

        [Output]
        public ITaskItem[] FileWrites { get; private set; } = Array.Empty<ITaskItem>();

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            var outputs = new List<ITaskItem>();
            var writes = new List<ITaskItem>();
            var generator = new ImageResizeGenerator(config);
            foreach (var image in Images.Select(x => x.ToOutputImage()))
            {
                var dependencies = SourceFiles.Select(x => x.ItemSpec).Concat(new[]
                {
                    image.InputFile, image.Watermark?.SourceFile, image.Watermark?.FontFile,
                    File.Exists(ConfigurationPath) ? ConfigurationPath : null
                });
                var fingerprint = ImageFingerprint.Create(new { Image = image, config.Configuration }, dependencies);
                if (!ImageFingerprint.IsCurrent(image.OutputFile, fingerprint))
                {
                    generator.ProcessImage(image);
                    File.WriteAllText(image.OutputFile + ".inputs", fingerprint);
                }
                outputs.Add(image.ToTaskItem());
                writes.Add(new TaskItem(image.OutputFile));
                writes.Add(new TaskItem(image.OutputFile + ".inputs"));
            }
            GeneratedImages = outputs.ToArray();
            FileWrites = writes.ToArray();
        }
    }
}
