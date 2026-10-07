using System.Drawing;
using Mobile.BuildTools.Models.AppIcons;
using SkiaSharp;

namespace Mobile.BuildTools.Drawing
{
    internal static class Watermark
    {
        public static ImageBase Create(WatermarkConfiguration configuration, PointF originalScale)
            => configuration switch
            {
                null => new EmptyWatermark(),
                _ => !string.IsNullOrEmpty(configuration.SourceFile)
                    ? new WatermarkImage(configuration.SourceFile)
                    : new WatermarkTextBanner(configuration, originalScale)
            };
    }

}
