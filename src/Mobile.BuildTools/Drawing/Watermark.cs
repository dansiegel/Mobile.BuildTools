using System;
using System.IO;
using System.Linq;
using System.Reflection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Mobile.BuildTools.Models.AppIcons;
using SixLabors.Fonts;
using SixLabors.Primitives;
using SixLabors.Shapes;

namespace Mobile.BuildTools.Drawing
{
    public static class Watermark
    {
        public static IImageProcessingContext ApplyWatermark(this IImageProcessingContext processingContext, WatermarkConfiguration config)
        {
            if (config is null)
            {
                return processingContext;
            }
            else if (!string.IsNullOrEmpty(config.SourceFile))
            {
                return processingContext.ApplyWatermark(config.SourceFile, config.Opacity);
            }

            var settings = WatermarkSettings.FromConfig(config);
            var size = processingContext.GetCurrentSize();
            // Create a new image for the watermark layer.
            using var watermark = new Image<Rgba32>(size.Width, size.Height);

            var xOffeset = 0;
            var yOffeset = 0;

            watermark.Mutate(ctx =>
            {
                // Draw the watermark.
                ctx.DrawWatermark(settings);

                var angle = 0.0f;
                if ((settings.Position == WatermarkPosition.BottomLeft) || (settings.Position == WatermarkPosition.TopRight))
                {
                    angle = 45.0f;
                    // Calculate the x/y offsets for later when we draw the watermark on top of the source image.
                    xOffeset = -(int)((Math.Sin(angle) * (size.Width / 2)) / 2);
                    yOffeset = -(int)((Math.Sin(angle) * (size.Height / 2)) / 2);
                }
                else if ((settings.Position == WatermarkPosition.BottomRight) || (settings.Position == WatermarkPosition.TopLeft))
                {
                    angle = -45.0f;
                    // Calculate the x/y offsets for later when we draw the watermark on top of the source image.
                    xOffeset = (int)((Math.Sin(angle) * (size.Width / 2)) / 2);
                    yOffeset = (int)((Math.Sin(angle) * (size.Height / 2)) / 2);
                }
                if (angle != 0)
                {
                    ctx.Rotate(angle);
                }

            });

            // Draw the watermark layer on top of the source image.
            processingContext.DrawImage(watermark, new Point(xOffeset, yOffeset), 1);

            return processingContext;
        }

        public static IImageProcessingContext ApplyWatermark(this IImageProcessingContext processingContext, string watermarkFile, double? opacity)
        {
            if (File.Exists(watermarkFile))
            {
                using var watermarkImage = Image.Load(watermarkFile);
                var size = processingContext.GetCurrentSize();
                watermarkImage.Mutate(x => x.Resize(size));
                processingContext.DrawImage(watermarkImage, PixelColorBlendingMode.Normal, (float)(opacity ?? 0.95));
            }

            return processingContext;
        }
        // Colors: Red,Blue,Purple,#006666
        private static IImageProcessingContext DrawWatermark(this IImageProcessingContext context, WatermarkSettings settings)
        {
            var imgSize = context.GetCurrentSize();

            // measure the text size
            var size = TextMeasurer.Measure(settings.Text, new RendererOptions(settings.TextFont));

            //find out how much we need to scale the text to fill the space (up or down)
            var scalingFactor = Math.Min(imgSize.Width / size.Width, imgSize.Height / size.Height) / 3;

            //create a new settings.TextFont
            var scaledFont = new Font(settings.TextFont, scalingFactor * settings.TextFont.Size);
            var center = settings.Position switch
            {
                WatermarkPosition.Top => new PointF(imgSize.Width / 2, (imgSize.Height / 9)),
                WatermarkPosition.TopLeft => new PointF(imgSize.Width / 2, (imgSize.Height / 9)),
                WatermarkPosition.TopRight => new PointF(imgSize.Width / 2, (imgSize.Height / 9)),
                _ => new PointF(imgSize.Width / 2, imgSize.Height - (imgSize.Height / 9)),
            };

            var textGraphicOptions = new TextGraphicsOptions(true)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            // Apply Banner
            context.DrawBanner(settings)
                .DrawText(textGraphicOptions, settings.Text, scaledFont, settings.TextColor, center);

            return context;
        }

        private static IImageProcessingContext DrawBanner(this IImageProcessingContext context, WatermarkSettings settings)
        {
            var imgSize = context.GetCurrentSize();

            var options = new GraphicsOptions(true, PixelColorBlendingMode.Normal, PixelAlphaCompositionMode.SrcOver, 1);

            var points = new[] { new PointF(0, imgSize.Height), new PointF(imgSize.Width, imgSize.Height) };

            var stop = 0;

            IBrush brush = new LinearGradientBrush(
                points[0],
                points[1],
                GradientRepetitionMode.Repeat,
                settings.Colors.Select(x => new ColorStop(stop++, x)).ToArray());

            var thickness = imgSize.Height / 4.5;

            var center = imgSize.Height - thickness;

            if ((settings.Position == WatermarkPosition.Top) || (settings.Position == WatermarkPosition.TopLeft) || (settings.Position == WatermarkPosition.TopRight))
            {
                points = new[] { new PointF(0, 0), new PointF(imgSize.Width, 0) };
            }

            System.Diagnostics.Debugger.Break();
            var fullThickness = (float)(thickness * 2);
            var pen = new Pen(brush, fullThickness);
            var polygon = new Polygon(new LinearLineSegment(points));

            return context.Draw(options, pen, polygon);
        }
    }
}
