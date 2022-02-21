using System.Drawing;
using SkiaSharp;
using Svg.Skia;

namespace Mobile.BuildTools.Drawing
{
    internal class VectorImage : ImageBase
    {
        private SKSvg svg;

        public VectorImage(string filename) : base(filename)
        {
            svg = new SKSvg();
            svg.Load(filename);
        }

        public override bool HasTransparentBackground
        {
            get
            {
                var size = GetOriginalSize();

                using var tempBitmap = new SKBitmap(size.Width, size.Height);
                using var canvas = new SKCanvas(tempBitmap);

                canvas.Clear(SKColors.Transparent);
                canvas.Save();

                canvas.DrawPicture(svg.Picture);

                return tempBitmap.HasTransparentBackground();
            }
        }

        public override Size GetOriginalSize() =>
            new Size((int)svg.Picture.CullRect.Size.Width, (int)svg.Picture.CullRect.Size.Height);

        public override void Draw(SKCanvas canvas, Context context)
        {
            SKPaint opacityPaint = null;
            if (context.Opacity != 1d)
            {
                opacityPaint = new SKPaint
                {
                    Color = SKColors.Transparent.WithAlpha((byte)(0xFF * context.Opacity))
                };
            }

            if (context.Scale.X >= 1)
            {
                // draw using default scaling
                canvas.DrawPicture(svg.Picture, opacityPaint);
            }
            else
            {
                // draw using raster downscaling
                var size = GetOriginalSize();

                // TODO: Try and apply a sensible default of 1024x1024 if no sizing information exists.
                // Log warning out if reverting to defaults.

                // vector scaling has rounding issues, so first draw as intended
                var info = new SKImageInfo(size.Width, size.Height);
                using var bmp = new SKBitmap(info);
                using var cvn = new SKCanvas(bmp);

                // draw to a larger canvas first
                cvn.Clear(context.BackgroundColor);
                cvn.DrawPicture(svg.Picture, opacityPaint);

                // set the paint to be the highest quality it can find
                var paint = new SKPaint
                {
                    IsAntialias = true,
                    FilterQuality = SKFilterQuality.High
                };

                // draw to the main canvas using the correct quality settings
                canvas.DrawBitmap(bmp, 0, 0, paint);
            }

            opacityPaint?.Dispose();
        }

        public override void Dispose()
        {
            svg?.Dispose();
            svg = null;
        }
    }
}
