using System.Drawing;
using System.Linq;
using Mobile.BuildTools.Models.AppIcons;
using SkiaSharp;

namespace Mobile.BuildTools.Drawing
{
    internal class Watermark : ImageBase
    {
        private readonly WatermarkConfiguration configuration;
        private readonly PointF originalScale;
        private readonly ImageBase sourceImage;

        public Watermark(WatermarkConfiguration configuration, PointF originalScale) : base(configuration?.SourceFile)
        {
            this.configuration = configuration;
            this.originalScale = originalScale;
            if (!string.IsNullOrEmpty(configuration?.SourceFile))
            {
                sourceImage = ImageBase.Load(configuration.SourceFile);
            }
        }

        public override bool HasTransparentBackground => sourceImage?.HasTransparentBackground ?? true;

        public override void Draw(SKCanvas canvas, Context context)
        {
            if (configuration is null)
            {
                return;
            }

            if (sourceImage != null)
            {
                if (!context.Scale.X.IsEqualTo(1f) ||
                    !context.Scale.Y.IsEqualTo(1f))
                {
                    context.Log.LogWarning("Watermark image has been scaled to meet output dimensions.");
                }

                canvas.Scale(context.Scale.X, context.Scale.Y);
                sourceImage.Draw(canvas, context);
                return;
            }

            DrawTextBanner(canvas, context);
        }

        public override Size GetOriginalSize() => sourceImage?.GetOriginalSize() ?? Size.Empty;

        public override void Dispose() => sourceImage?.Dispose();

        private void DrawTextBanner(SKCanvas canvas, Context context)
        {
            // Undo the original scaling factor to simplify the rendering.
            canvas.Scale(originalScale.X, originalScale.Y);

            var settings = WatermarkSettings.FromConfig(configuration);
            var bannerHeight = (float)(context.Size.Width / 4.5);
            var (start, end) = GetBannerLocations(settings.Position, context.Size, (float)bannerHeight);

            var shader = SKShader.CreateLinearGradient(
                start,
                end,
                settings.Colors.Select(c => c.WithAlpha((byte)(0xFF * context.Opacity))).ToArray(),
                null,
                SKShaderTileMode.Clamp);
            using var linePaint = new SKPaint
            {
                Shader = shader,
                IsStroke = true,
                StrokeWidth = bannerHeight,
                Style = SKPaintStyle.Stroke
            };
            var path = new SKPath();
            path.MoveTo(start);
            path.LineTo(end);

            canvas.DrawPath(path, linePaint);
            using var textPaint = new SKPaint
            {
                Color = settings.TextColor.WithAlpha((byte)(0xFF * context.Opacity)),
                TextAlign = SKTextAlign.Center,
                TextSize = context.Size.Width / 5, // TODO: calculate a more appropriate size.
                Typeface = settings.Typeface
            };
            canvas.DrawTextOnPath(settings.Text, path, 0, 0, textPaint);
        }

        private static (SKPoint, SKPoint) GetBannerLocations(WatermarkPosition watermarkPosition, Size size, float bannerHeight)
            => watermarkPosition switch
            {
                WatermarkPosition.Top => (new SKPoint(0, 0), new SKPoint(size.Width, 0)),
                WatermarkPosition.Bottom => (new SKPoint(0, size.Height), new SKPoint(size.Width, size.Height)),
                WatermarkPosition.TopLeft => (new SKPoint(-bannerHeight, size.Height / 2 + bannerHeight), new SKPoint(size.Width / 2 + bannerHeight, -bannerHeight)),
                WatermarkPosition.TopRight => (new SKPoint(size.Width / 2 - bannerHeight, -bannerHeight), new SKPoint(size.Width + bannerHeight, size.Height / 2 + bannerHeight)),
                WatermarkPosition.BottomLeft => (new SKPoint(-bannerHeight, size.Height / 2 - bannerHeight), new SKPoint(size.Width / 2 + bannerHeight, size.Height + bannerHeight)),
                WatermarkPosition.BottomRight => (new SKPoint(size.Width / 2 - bannerHeight, size.Height + bannerHeight), new SKPoint(size.Width + bannerHeight, size.Height / 2 - bannerHeight)),
                _ => (new SKPoint(0, 0), new SKPoint(0, 0)),
            };
    }
}
