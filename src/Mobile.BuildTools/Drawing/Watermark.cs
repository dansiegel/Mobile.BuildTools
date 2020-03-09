using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Mobile.BuildTools.Drawing
{
    public static class Watermark
    {
        public static IImageProcessingContext ApplyWatermark(this IImageProcessingContext processingContext, string watermarkFile, double? opacity)
        {
            if(File.Exists(watermarkFile))
            {
                using var watermarkImage = Image.Load(watermarkFile);
                var size = processingContext.GetCurrentSize();
                watermarkImage.Mutate(x => x.Resize(size));
                processingContext.DrawImage(watermarkImage, PixelColorBlendingMode.Normal, (float)(opacity ?? 0.95));
            }

            return processingContext;
        }
    }
}
