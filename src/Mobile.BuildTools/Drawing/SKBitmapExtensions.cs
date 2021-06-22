using SkiaSharp;

namespace Mobile.BuildTools.Drawing
{
    public static class SKBitmapExtensions
    {
        public static bool HasTransparentBackground(this SKBitmap bitmap)
        {
            var imageWidth = bitmap.Info.Width;
            var imageHeight = bitmap.Info.Height;

            for (var x = 0; x < imageWidth; x++)
            {
                for (var y = 0; y < imageHeight; y++)
                {
                    if (bitmap.GetPixel(x, y).Alpha == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
