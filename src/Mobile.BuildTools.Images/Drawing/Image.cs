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

        public override bool HasTransparentBackground => bitmap.HasTransparentBackground();

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
