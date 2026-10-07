using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Drawing;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models.AppIcons;
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
            Log.LogMessage($"Generating file '{outputImage.OutputFile}'");
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputImage.OutputFile)));
            using var image = ImageBase.Load(outputImage.InputFile);
            var context = CreateContext(
                GetBackgroundColor(outputImage.BackgroundColor, outputImage.RequiresBackgroundColor && image.HasTransparentBackground),
                Log, 1, outputImage.Scale, outputImage.Width, outputImage.Height, image.GetOriginalSize());

            if (!context.Scale.X.IsEqualTo(context.Scale.Y))
                Log.LogWarning("Image aspect ratio is not being maintained.");

            var padding = outputImage.PaddingFactor.GetValueOrDefault(1);
            if (padding == 0)
                padding = 1;
            if (double.IsNaN(padding) || double.IsInfinity(padding) || padding < 0)
                throw new ArgumentOutOfRangeException(nameof(outputImage.PaddingFactor), "Padding factor must be positive.");

            using var bitmap = new SKBitmap(context.Size.Width, context.Size.Height);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(padding != 1
                ? GetBackgroundColor(outputImage.PaddingColor, outputImage.RequiresBackgroundColor && image.HasTransparentBackground)
                : context.BackgroundColor);

            // Padding fits the content inside the requested dimensions, never enlarging the output.
            var contentWidth = (float)(context.Size.Width / padding);
            var contentHeight = (float)(context.Size.Height / padding);
            var left = (context.Size.Width - contentWidth) / 2;
            var top = (context.Size.Height - contentHeight) / 2;
            if (padding != 1)
            {
                using var background = new SKPaint { Color = context.BackgroundColor };
                canvas.DrawRect(left, top, contentWidth, contentHeight, background);
            }

            canvas.Save();
            canvas.Translate(left, top);
            canvas.Scale(context.Scale.X / (float)padding, context.Scale.Y / (float)padding);
            image.Draw(canvas, context);
            canvas.Restore();

            // Watermarks use final output coordinates; source-image scaling must not affect them.
            ApplyWatermark(outputImage, context, Log, canvas);
            using var stream = File.Create(outputImage.OutputFile);
            bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
        }

        private static void ApplyWatermark(OutputImage outputImage, Context context, ILog log, SKCanvas canvas)
        {
            if (outputImage.Watermark is null)
                return;
            var opacity = outputImage.Watermark.Opacity ?? 1;
            if (opacity < 0 || opacity > 1 || double.IsNaN(opacity))
                throw new ArgumentOutOfRangeException(nameof(outputImage.Watermark.Opacity), "Watermark opacity must be between zero and one.");
            using var watermark = Watermark.Create(outputImage.Watermark, new PointF(1, 1));
            var original = watermark.GetOriginalSize();
            var watermarkContext = original.IsEmpty
                ? new Context(SKColors.Transparent, log, opacity, context.Size.Width, context.Size.Height, 1)
                : CreateContext(SKColors.Transparent, log, opacity, 0, context.Size.Width, context.Size.Height, original);
            canvas.Save();
            watermark.Draw(canvas, watermarkContext);
            canvas.Restore();
        }

        private static SKColor GetBackgroundColor(string colorText, bool requiresBackgroundColor)
        {
            if ((!string.IsNullOrWhiteSpace(colorText) || requiresBackgroundColor) &&
                ColorUtils.TryParse(string.IsNullOrWhiteSpace(colorText) ? Constants.DefaultBackgroundColor : colorText, out var color))
                return color;
            return SKColors.Transparent;
        }

        private static Context CreateContext(SKColor backgroundColor, ILog log, double opacity, double scale, int width, int height, Size currentSize)
        {
            if (currentSize.Width <= 0 || currentSize.Height <= 0)
                throw new InvalidDataException("Source image dimensions must be greater than zero.");
            if (double.IsNaN(scale) || double.IsInfinity(scale) || scale < 0 || width < 0 || height < 0)
                throw new ArgumentOutOfRangeException(nameof(scale), "Image dimensions and scale must be finite and nonnegative.");

            // Explicit dimensions win over Scale. A single dimension preserves the source aspect ratio.
            if (width > 0 && height == 0)
                height = Math.Max(1, (int)Math.Round((double)currentSize.Height * width / currentSize.Width));
            else if (height > 0 && width == 0)
                width = Math.Max(1, (int)Math.Round((double)currentSize.Width * height / currentSize.Height));
            else if (width == 0 && height == 0)
            {
                if (scale == 0)
                    scale = 1;
                width = Math.Max(1, (int)Math.Round(currentSize.Width * scale));
                height = Math.Max(1, (int)Math.Round(currentSize.Height * scale));
            }
            return new Context(backgroundColor, log, opacity, width, height,
                (float)width / currentSize.Width, (float)height / currentSize.Height);
        }
    }
}
