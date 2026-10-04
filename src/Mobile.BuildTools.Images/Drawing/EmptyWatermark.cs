using System.Drawing;
using SkiaSharp;

namespace Mobile.BuildTools.Drawing
{
    internal sealed class EmptyWatermark : ImageBase
    {
        public EmptyWatermark() : base(string.Empty) { }
        public override bool HasTransparentBackground => false;
        public override void Draw(SKCanvas canvas, Context context) { }
        public override Size GetOriginalSize() => Size.Empty;
    }
}
