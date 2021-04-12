using System.Drawing;

namespace Mobile.BuildTools.Drawing
{
    public class OutputSize
    {
        public PointF Scale { get; }

        public Size Size { get; }

        public OutputSize(int width, int height, float uniformScale) : this(width, height, uniformScale, uniformScale)
        {
        }

        public OutputSize(int width, int height, float xScale, float yScale) : this(new Size(width, height), new PointF(xScale, yScale))
        {
        }

        private OutputSize(Size size, PointF scale)
        {
            Size = size;
            Scale = scale;
        }
    }
}
