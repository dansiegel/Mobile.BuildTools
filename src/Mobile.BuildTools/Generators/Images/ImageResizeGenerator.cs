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
                var backgroundColor = GetBackgroundColor(outputImage, image.HasTransparentBackground);
                var context = CreateContext(backgroundColor, Log, 1.0, outputImage.Scale, outputImage.Width, outputImage.Height, image.GetOriginalSize());

                if (!context.Scale.X.IsEqualTo(context.Scale.Y))
                {
                    Log.LogWarning("Image aspect ratio is not being maintained.");
                }

                using var tempBitmap = new SKBitmap(context.Size.Width, context.Size.Height);
                using var canvas = new SKCanvas(tempBitmap);

                canvas.Clear(backgroundColor);
                canvas.Save();
                canvas.Scale(context.Scale.X, context.Scale.Y);

                image.Draw(canvas, context);

                // Apply watermark
                if (outputImage.Watermark != null)
                {
                    using var watermark = Watermark.Create(outputImage.Watermark, new PointF(1 / context.Scale.X, 1 / context.Scale.Y));
                    var watermarkContext = CreateContext(SKColors.Transparent, Log, outputImage.Watermark.Opacity ?? 1.0, 0, context.Size.Width, context.Size.Height, watermark.GetOriginalSize());
                    watermark.Draw(canvas, watermarkContext);
                }

                // TODO: apply padding.

                using var stream = File.Create(outputImage.OutputFile);
                tempBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
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

        private static Context CreateContext(SKColor backgroundColor, Logging.ILog log, double opacity, double scale, int width, int height, Size currentSize)
        {
            if (scale > 0)
            {
                return new Context(
                    backgroundColor,
                    log,
                    opacity,
                    (int)(currentSize.Width * scale),
                    (int)(currentSize.Height * scale),
                    (float)scale);
            }

            return new Context(
                backgroundColor,
                log,
                opacity,
                width,
                height,
                (float)width / currentSize.Width,
                (float)height / currentSize.Height);
        }
    }
}
