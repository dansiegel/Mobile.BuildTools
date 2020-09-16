namespace Mobile.BuildTools.Models.AppIcons
{
    public interface IImageResource
    {
        string SourceFile { get; }
        string BackgroundColor { get; }
        int? Height { get; }
        bool Ignore { get; }
        string Name { get; }
        string PaddingColor { get; }
        double? PaddingFactor { get; }
#if SCHEMAGENERATOR
        string ResourceType { get; }
#else
        PlatformResourceType ResourceType { get; }
#endif
        double Scale { get; }
        WatermarkConfiguration Watermark { get; }
        int? Width { get; }
    }

    internal sealed class ImageResource : IImageResource
    {
        public string SourceFile { get; set; }
        public string BackgroundColor { get; set; }
        public int? Height { get; set; }
        public bool Ignore { get; set; }
        public string Name { get; set; }
        public string PaddingColor { get; set; }
        public double? PaddingFactor { get; set; }
#if SCHEMAGENERATOR
        public string ResourceType { get; set; }
#else
        public PlatformResourceType ResourceType { get; set; }
#endif
        public double Scale { get; set; }
        public WatermarkConfiguration Watermark { get; set; }
        public int? Width { get; set; }
    }
}