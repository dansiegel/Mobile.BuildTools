﻿using System.Drawing;
using System.Linq;
using Mobile.BuildTools.Models.AppIcons;
using SkiaSharp;

namespace Mobile.BuildTools.Drawing
{
    internal class WatermarkTextBanner : ImageBase
    {
        private readonly string[] commonDescenders = new[] { "g", "j", "p", "q", "y" };
        private readonly WatermarkConfiguration configuration;
        private readonly PointF originalScale;

        public WatermarkTextBanner(WatermarkConfiguration configuration, PointF originalScale) : base(string.Empty)
        {
            this.configuration = configuration;
            this.originalScale = originalScale;
        }

        public override bool HasTransparentBackground => true;

        public override void Draw(SKCanvas canvas, Context context)
        {
            DrawTextBanner(canvas, context);
        }

        public override Size GetOriginalSize() => Size.Empty;

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
                Typeface = settings.Typeface
            };

            // Adjust TextSize property so text is 45% of the resulting image size.
            var textWidth = textPaint.MeasureText(settings.Text);
            textPaint.TextSize = 0.45f * context.Size.Width * textPaint.TextSize / textWidth;

            // Find the text bounds 
            var textBounds = new SKRect();
            // It may well be a Skia bug but it appears the measuring doesn't include the height of the descender characters in the measured height.
            textPaint.MeasureText(StripDescenders(settings.Text), ref textBounds);

            // Text drawn on the centre of the line so we need to bump it down to align the centre of the text.
            canvas.DrawTextOnPath(settings.Text, path, 0, textBounds.Height / 2, textPaint);
        }

        private string StripDescenders(string text)
        {
            // TODO: Surely there must be a better way?
            var cleansedText = text;
            foreach (var descender in commonDescenders)
            {
                cleansedText = cleansedText.Replace(descender, "");
            }

            if (cleansedText.Length == 0)
            {
                cleansedText = "dev";
            }

            return cleansedText;
        }

        private static (SKPoint, SKPoint) GetBannerLocations(WatermarkPosition watermarkPosition, Size size, float bannerHeight)
            => watermarkPosition switch
            {
                WatermarkPosition.Top => (new SKPoint(0, bannerHeight / 2), new SKPoint(size.Width, bannerHeight / 2)),
                WatermarkPosition.Bottom => (new SKPoint(0, size.Height - (bannerHeight / 2)), new SKPoint(size.Width, size.Height - (bannerHeight / 2))),
                WatermarkPosition.TopLeft => (new SKPoint(-bannerHeight, size.Height / 2 + bannerHeight), new SKPoint(size.Width / 2 + bannerHeight, -bannerHeight)),
                WatermarkPosition.TopRight => (new SKPoint(size.Width / 2 - bannerHeight, -bannerHeight), new SKPoint(size.Width + bannerHeight, size.Height / 2 + bannerHeight)),
                WatermarkPosition.BottomLeft => (new SKPoint(-bannerHeight, size.Height / 2 - bannerHeight), new SKPoint(size.Width / 2 + bannerHeight, size.Height + bannerHeight)),
                WatermarkPosition.BottomRight => (new SKPoint(size.Width / 2 - bannerHeight, size.Height + bannerHeight), new SKPoint(size.Width + bannerHeight, size.Height / 2 - bannerHeight)),
                _ => (new SKPoint(0, 0), new SKPoint(0, 0)),
            };
    }
}
