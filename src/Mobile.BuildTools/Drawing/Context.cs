using System.Drawing;
using Mobile.BuildTools.Logging;
using SkiaSharp;

namespace Mobile.BuildTools.Drawing
{
    internal class Context
    {
        public SKColor BackgroundColor { get; }

        public ILog Log { get; }

        public double Opacity { get; } = 1.0;

        public PointF Scale { get; }

        public Size Size { get; }

        public Context(SKColor backgroundColor, ILog log, double opacity, int width, int height, float uniformScale) : this(backgroundColor, log, opacity, width, height, uniformScale, uniformScale)
        {
        }

        public Context(SKColor backgroundColor, ILog log, double opacity, int width, int height, float xScale, float yScale) : this(backgroundColor, log, opacity, new Size(width, height), new PointF(xScale, yScale))
        {
        }

        private Context(SKColor backgroundColor, ILog log, double opacity, Size size, PointF scale)
        {
            BackgroundColor = backgroundColor;
            Opacity = opacity;
            Log = log;
            Size = size;
            Scale = scale;
        }
    }
}
