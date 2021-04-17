using System.Drawing;
using SkiaSharp;

namespace Mobile.BuildTools.Drawing
{
    internal class Image : ImageBase
    {
        private SKBitmap bitmap;

        public Image(string filename) : base(filename)
        {
            bitmap = SKBitmap.Decode(filename);
        }

        public override bool HasTransparentBackground
        {
            get
            {
                var imageWidth = GetOriginalSize().Width;
                var imageHeight = GetOriginalSize().Height;

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

        public override Size GetOriginalSize() =>
            new Size(bitmap.Info.Width, bitmap.Info.Height);

        public override void Draw(SKCanvas canvas, Context context)
        {
            SKPaint paint = null;
            if (context.Opacity != 1d)
            {
                paint = new SKPaint
                {
                    Color = SKColors.Transparent.WithAlpha((byte)(0xFF * context.Opacity))
                };
            }

            canvas.DrawBitmap(bitmap, 0, 0, paint);

            paint?.Dispose();
        }

        public override void Dispose()
        {
            bitmap?.Dispose();
            bitmap = null;
        }
    }
}
