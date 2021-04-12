using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Drawing;
using Mobile.BuildTools.Models.AppIcons;
using Newtonsoft.Json;
using SkiaSharp;

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

            using var image = ImageBase.Load(outputImage.InputFile);

            try
            {
                var outputSize = GetOutputSize(outputImage, image.GetOriginalSize());

                if (Math.Abs(outputSize.Scale.X - outputSize.Scale.Y) >= 0.0001)
                {
                    Log.LogWarning("Image aspect ratio is not being maintained.");
                }

                // Allocate
                using (var tempBitmap = new SKBitmap(outputSize.Size.Width, outputSize.Size.Height))
                {
                    // Draw (copy)
                    using (var canvas = new SKCanvas(tempBitmap))
                    {
                        var backgroundColor = GetBackgroundColor(outputImage, image.HasTransparentBackground);
                        canvas.Clear(backgroundColor);
                        canvas.Save();
                        canvas.Scale(outputSize.Scale.X, outputSize.Scale.Y);

                        image.Draw(canvas, outputSize.Scale.X, backgroundColor);

                        // TODO: apply watermark
                        // TODO: apply padding.
                    }

                    // Save (encode)
                    using var stream = File.Create(outputImage.OutputFile);
                    tempBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
                }
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
        }

        private static SKColor GetBackgroundColor(OutputImage outputImage, bool hasTransparentBackground)
        {
            var requiresBackground = outputImage.RequiresBackgroundColor && hasTransparentBackground;
            var isExpectedBackgroundDefined = !string.IsNullOrWhiteSpace(outputImage.BackgroundColor);

            if (isExpectedBackgroundDefined || requiresBackground)
            {
                if (ColorUtils.TryParse(isExpectedBackgroundDefined ? outputImage.BackgroundColor : Constants.DefaultBackgroundColor, out var color))
                {
                    return color;
                }
            }

            return SKColors.Transparent;
        }

        private static OutputSize GetOutputSize(OutputImage output, Size currentSize)
        {
            if (output.Scale > 0)
            {
                return new OutputSize(
                    (int)(currentSize.Width * output.Scale),
                    (int)(currentSize.Height * output.Scale),
                    (float)output.Scale);
            }

            return new OutputSize(
                output.Width,
                output.Height,
                (float)output.Width / currentSize.Width,
                (float)output.Height / currentSize.Height);
        }
    }
}
