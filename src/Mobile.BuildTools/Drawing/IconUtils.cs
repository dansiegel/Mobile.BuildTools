using System;
using System.Globalization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Mobile.BuildTools.Drawing
{
    public static class IconUtils
    {
        public static bool HasTransparentBackground(this Image image)
        {
            using var clone = image.CloneAs<Rgba32>();
            for (var y = 0; y < image.Height; ++y)
            {
                var pixelRowSpan = clone.GetPixelRowSpan(y);
                for (var x = 0; x < image.Width; ++x)
                {
                    if (pixelRowSpan[x].A == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void ApplyBackground(this IImageProcessingContext context, string hexColor)
        {
            var color = ColorUtils.TryParse(hexColor, out var c) ? c : Color.ParseHex(Constants.DefaultBackgroundColor);

            context.BackgroundColor(color);
        }

        public static void ResizeCanvas(this IImageProcessingContext context, double? pad, string padColorNameOrHexString)
        {
            if (pad is null || pad.Value < 1) return;

            var scale = (double)pad;
            var color = ColorUtils.TryParse(padColorNameOrHexString, out var result) ? result : Color.Transparent;

            var size = context.GetCurrentSize();
            var height = (int)(size.Height * scale);
            var width = (int)(size.Width * scale);
            context.Pad(width, height, color);
        }
    }
}
