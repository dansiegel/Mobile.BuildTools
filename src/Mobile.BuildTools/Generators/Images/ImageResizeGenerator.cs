using System.Collections.Generic;
using System.IO;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Drawing;
using Mobile.BuildTools.Models.AppIcons;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Mobile.BuildTools.Generators.Images
{
    internal class ImageResizeGenerator : GeneratorBase
    {
        public ImageResizeGenerator(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        public IEnumerable<OutputImage> OutputImages { get; set; }
        public string IntermediateOutputDirectory { get; set; }
        public double? WatermarkOpacity { get; set; }

        protected override void ExecuteInternal()
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Launch();
#endif
            foreach(var outputImage in OutputImages)
            {
                Log.LogMessage($"Generating file '{outputImage.OutputFile}");
                Directory.CreateDirectory(Path.Combine(IntermediateOutputDirectory, Directory.GetParent(outputImage.OutputFile).FullName));
                using var image = Image.Load(outputImage.InputFile);
                if (outputImage.Scale != 1)
                {
                    image.Mutate(x =>
                    {
                        x.Resize(GetUpdatedSize(outputImage, image));
                        if (!string.IsNullOrEmpty(outputImage.WatermarkFilePath))
                        {
                            x.ApplyWatermark(outputImage.WatermarkFilePath, WatermarkOpacity);
                        }
                    });

                    if (outputImage.RequiresBackgroundColor && image.HasTransparentBackground())
                    {
                        // TODO: Make this a configuration.
                        image.ApplyBackground("#FFFFFF");
                    }
                }

                using var outputStream = new FileStream(outputImage.OutputFile, FileMode.OpenOrCreate);
                image.SaveAsPng(outputStream);
            }
        }

        private static Size GetUpdatedSize(OutputImage output, Image image)
        {
            if (output.Scale > 0)
            {
                return new Size((int)(image.Width * output.Scale), (int)(image.Height * output.Scale));
            }

            return new Size(output.Width, output.Height);
        }
    }
}
