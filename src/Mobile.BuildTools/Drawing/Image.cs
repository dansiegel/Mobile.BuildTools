using System.Drawing;
using SkiaSharp;

namespace Mobile.BuildTools.Drawing
{
    internal class Image : ImageBase
    {
        private SKBitmap bmp;

        public Image(string filename) : base(filename)
        {
            bmp = SKBitmap.Decode(filename);
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
                        if (bmp.GetPixel(x, y).Alpha == 0)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public override Size GetOriginalSize() =>
            new Size(bmp.Info.Width, bmp.Info.Height);

        public override void Draw(SKCanvas canvas, float scale, SKColor backgroundColor) => canvas.DrawBitmap(bmp, 0, 0, Paint);

        public override void Dispose()
        {
            bmp?.Dispose();
            bmp = null;
        }
    }
}
