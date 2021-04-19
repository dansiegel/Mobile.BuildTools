using System.Drawing;
using SkiaSharp;

namespace Mobile.BuildTools.Drawing
{
    internal class WatermarkImage : ImageBase
    {
        private readonly ImageBase sourceImage;

        public WatermarkImage(string filename) : base(filename)
        {
            sourceImage = ImageBase.Load(filename);
        }

        public override bool HasTransparentBackground => sourceImage.HasTransparentBackground;

        public override void Draw(SKCanvas canvas, Context context)
        {
            if (!context.Scale.X.IsEqualTo(1f) ||
                !context.Scale.Y.IsEqualTo(1f))
            {
                context.Log.LogWarning("Watermark image has been scaled to meet output dimensions.");
            }

            canvas.Scale(context.Scale.X, context.Scale.Y);
            sourceImage.Draw(canvas, context);
        }

        public override Size GetOriginalSize() => sourceImage?.GetOriginalSize() ?? Size.Empty;

        public override void Dispose() => sourceImage?.Dispose();
    }
}
