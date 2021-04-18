using System.Drawing;
using Mobile.BuildTools.Models.AppIcons;
using SkiaSharp;

namespace Mobile.BuildTools.Drawing
{
    internal abstract class Watermark : ImageBase
    {
        public Watermark(string filename) : base(filename)
        {
        }

        public static Watermark Create(WatermarkConfiguration configuration, PointF originalScale)
            => configuration switch
            {
                null => new EmptyWatermark(),
                _ => !string.IsNullOrEmpty(configuration.SourceFile)
                    ? new WatermarkImage(configuration.SourceFile)
                    : (Watermark)new WatermarkTextBanner(configuration, originalScale)
            };

        internal class EmptyWatermark : Watermark
        {
            public EmptyWatermark() : base(string.Empty)
            {
            }

            public override bool HasTransparentBackground => false;

            public override void Draw(SKCanvas canvas, Context context)
            {

            }

            public override Size GetOriginalSize() => Size.Empty;
        }
    }
}
