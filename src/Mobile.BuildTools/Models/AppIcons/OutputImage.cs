using System.Collections;

namespace Mobile.BuildTools.Models.AppIcons
{
    public partial class OutputImage : IEqualityComparer
    {
        public string InputFile { get; set; }

        public string OutputFile { get; set; }

        public string OutputLink { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public double Scale { get; set; }

        public bool RequiresBackgroundColor { get; set; }

        public bool ShouldBeVisible { get; set; }

        public string WatermarkFilePath { get; set; }

        public new bool Equals(object x, object y)
        {
            if (x is OutputImage image1 && y is OutputImage image2)
                return image1.OutputFile == image2.OutputFile;

            return x == y;
        }

        public int GetHashCode(object obj)
        {
            if (obj is OutputImage oi)
                return oi.OutputFile.GetHashCode();

            return obj.GetHashCode();
        }
    }
}
