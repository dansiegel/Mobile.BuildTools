using System.IO;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Models.AppIcons;
using SkiaSharp;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures.Generators
{
    public class ImageSizingRegressionFixture : FixtureBase
    {
        public ImageSizingRegressionFixture(ITestOutputHelper output) : base(output) { }

        [Theory]
        [InlineData(20, 10, 20, 10)]
        [InlineData(20, 0, 20, 10)]
        [InlineData(0, 10, 20, 10)]
        public void DimensionsOverrideScaleAndMissingDimensionPreservesAspectRatio(int width, int height, int expectedWidth, int expectedHeight)
        {
            var config = GetConfiguration();
            var source = Path.Combine(config.ProjectDirectory, "source.png");
            using (var bitmap = new SKBitmap(100, 50))
            {
                bitmap.Erase(SKColors.Blue);
                using var stream = File.Create(source);
                bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            }
            var output = Path.Combine(config.IntermediateOutputPath, "result.png");
            new ImageResizeGenerator(config).ProcessImage(new OutputImage { InputFile = source, OutputFile = output, Width = width, Height = height, Scale = 0.5 });
            using var result = SKBitmap.Decode(output);
            Assert.Equal(expectedWidth, result.Width);
            Assert.Equal(expectedHeight, result.Height);
        }

        [Fact]
        public void ImageWatermarkUsesOutputCoordinates()
        {
            var config = GetConfiguration();
            var source = Path.Combine(config.ProjectDirectory, "source.png");
            var watermark = Path.Combine(config.ProjectDirectory, "mark.png");
            using (var bitmap = new SKBitmap(100, 50))
            {
                bitmap.Erase(SKColors.Blue);
                using var stream = File.Create(source);
                bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            }
            using (var bitmap = new SKBitmap(2, 1))
            {
                bitmap.Erase(SKColors.Transparent);
                bitmap.SetPixel(0, 0, SKColors.Red);
                using var stream = File.Create(watermark);
                bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            }
            var output = Path.Combine(config.IntermediateOutputPath, "result.png");
            new ImageResizeGenerator(config).ProcessImage(new OutputImage
            {
                InputFile = source, OutputFile = output, Width = 20, Height = 10, Scale = 0.5,
                Watermark = new WatermarkConfiguration { SourceFile = watermark }
            });
            using var result = SKBitmap.Decode(output);
            Assert.Equal(SKColors.Red, result.GetPixel(2, 5));
            Assert.Equal(SKColors.Blue, result.GetPixel(18, 5));
        }
    }
}
