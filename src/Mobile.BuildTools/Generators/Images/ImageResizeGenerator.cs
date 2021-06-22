using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Drawing;
using Mobile.BuildTools.Logging;
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
            foreach (var outputImage in OutputImages)
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
                var context = CreateContext(
                    GetBackgroundColor(outputImage.BackgroundColor, outputImage.RequiresBackgroundColor && image.HasTransparentBackground),
                    Log,
                    1.0,
                    outputImage.Scale,
                    outputImage.Width,
                    outputImage.Height,
                    image.GetOriginalSize()); 

                if (!context.Scale.X.IsEqualTo(context.Scale.Y))
                {
                    Log.LogWarning("Image aspect ratio is not being maintained.");
                }

                using var tempBitmap = new SKBitmap(context.Size.Width, context.Size.Height);
                using var canvas = new SKCanvas(tempBitmap);

                canvas.Clear(context.BackgroundColor);
                canvas.Save();
                canvas.Scale(context.Scale.X, context.Scale.Y);

                image.Draw(canvas, context);

                ApplyWatermark(outputImage, context, Log, canvas);

                using var outputBitmap = ApplyPadding(outputImage, image, context, tempBitmap);

                using var stream = File.Create(outputImage.OutputFile);
                outputBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            }
            catch (System.Exception ex)
            {
#if DEBUG
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
                else if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    System.Diagnostics.Debugger.Launch();
#endif
                Log.LogWarning(@$"Encountered Fatal error while processing image:
{JsonConvert.SerializeObject(outputImage)}");
                throw;
            }
        }

        private SKBitmap ApplyPadding(OutputImage outputImage, ImageBase image, Context context, SKBitmap outputBitmap)
        {
            if (outputImage.PaddingFactor is null ||
                outputImage.PaddingFactor == 0)
            {
                return outputBitmap;
            }

            var paddedBitmap = new SKBitmap(context.Size.Width, context.Size.Height);
            using var paddedCanvas = new SKCanvas(paddedBitmap);

            var paddedContext = CreateContext(
                GetBackgroundColor(outputImage.PaddingColor, outputImage.RequiresBackgroundColor && image.HasTransparentBackground),
                Log,
                1.0,
                outputImage.PaddingFactor.Value,
                0,
                0,
                context.Size);

            paddedCanvas.Clear(paddedContext.BackgroundColor);
            paddedCanvas.Save();

            using var resizedBitmap = outputBitmap.Resize(new SKImageInfo(paddedContext.Size.Width, paddedContext.Size.Height), SKFilterQuality.High);

            paddedCanvas.DrawBitmap(
                resizedBitmap,
                (context.Size.Width - paddedContext.Size.Width) / 2,
                (context.Size.Height - paddedContext.Size.Height) / 2);

            return paddedBitmap;
        }

        private static void ApplyWatermark(OutputImage outputImage, Context context, ILog log, SKCanvas canvas)
        {
            using var watermark = Watermark.Create(outputImage.Watermark, new PointF(1 / context.Scale.X, 1 / context.Scale.Y));
            var watermarkContext = CreateContext(SKColors.Transparent, log, outputImage.Watermark?.Opacity ?? 1.0, 0, context.Size.Width, context.Size.Height, watermark.GetOriginalSize());
            watermark.Draw(canvas, watermarkContext);
        }

        private static SKColor GetBackgroundColor(string colorText, bool requiresBackgroundColor)
        {
            var isExpectedBackgroundDefined = !string.IsNullOrWhiteSpace(colorText);

            if (isExpectedBackgroundDefined || requiresBackgroundColor)
            {
                if (ColorUtils.TryParse(isExpectedBackgroundDefined ? colorText : Constants.DefaultBackgroundColor, out var color))
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
