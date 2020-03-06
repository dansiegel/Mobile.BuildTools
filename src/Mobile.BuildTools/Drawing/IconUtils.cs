using System;
using System.Linq;
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
            using (var clone = image.CloneAs<Rgba32>())
            {
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
            }

            return false;
        }

        public static void ApplyBackground(this Image image, string hexColor)
        {
            if (string.IsNullOrWhiteSpace(hexColor))
                hexColor = Constants.DefaultBackgroundColor;

            if (hexColor[0] != '#')
                hexColor = $"#{hexColor}";

            hexColor = hexColor.ToUpper();

            if (hexColor.Length != 7 || hexColor.Any(x => !char.IsLetterOrDigit(x)))
                throw new Exception($"An invalid hex color has been provided for the image. {hexColor}");

            image.Mutate(c =>
            {
                c.BackgroundColor(Color.FromHex(hexColor));
            });
        }
    }
}
