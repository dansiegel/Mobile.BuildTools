using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Drawing;
using Mobile.BuildTools.Models.AppIcons;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Mobile.BuildTools.Generators.Images
{
    internal class ImageResizeGenerator : GeneratorBase<IList<OutputImage>>
    {
        public ImageResizeGenerator(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        public IEnumerable<OutputImage> OutputImages { get; set; }

        protected override void ExecuteInternal()
        {
            Outputs = new List<OutputImage>();
            foreach(var outputImage in OutputImages)
            {
                ProcessImage(outputImage);
                Outputs.Add(outputImage);
            }
        }

        internal void ProcessImage(OutputImage outputImage)
        {
            Log.LogMessage($"Generating file '{outputImage.OutputFile}");
            var fi = new FileInfo(outputImage.OutputFile);
            Directory.CreateDirectory(Path.Combine(Build.IntermediateOutputPath, fi.DirectoryName));
            using var image = Image.Load(outputImage.InputFile);
            var requiresBackground = outputImage.RequiresBackgroundColor && image.HasTransparentBackground();

            try
            {
                image.Mutate(x => MutateImage(x, outputImage, requiresBackground));
            }
            catch (System.Exception ex)
            {
#if DEBUG
                if (!Debugger.IsAttached)
                    Debugger.Launch();
#endif
                Log.LogWarning(@$"Encountered Fatal error while processing image:
{JsonConvert.SerializeObject(outputImage)}");
                throw;
            }

            using var outputStream = new FileStream(outputImage.OutputFile, FileMode.OpenOrCreate);
            image.SaveAsPng(outputStream);
        }

        private static IImageProcessingContext MutateImage(IImageProcessingContext ctx, OutputImage outputImage, bool requiresBackground)
        {
            var currentSize = ctx.GetCurrentSize();
            ctx.Resize(GetUpdatedSize(outputImage, currentSize));
            ctx.ApplyWatermark(outputImage.Watermark);

            if (!string.IsNullOrWhiteSpace(outputImage.BackgroundColor) || requiresBackground)
            {
                ctx.ApplyBackground(outputImage.BackgroundColor);
            }

            ctx.ResizeCanvas(outputImage.PaddingFactor, outputImage.PaddingColor);
            return ctx;
        }

        private static Size GetUpdatedSize(OutputImage output, Size currentSize)
        {
            if (output.Scale > 0)
            {
                return new Size((int)(currentSize.Width * output.Scale), (int)(currentSize.Height * output.Scale));
            }

            return new Size(output.Width, output.Height);
        }
    }
}
