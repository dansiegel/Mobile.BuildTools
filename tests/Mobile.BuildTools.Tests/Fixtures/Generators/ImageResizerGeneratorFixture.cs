using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Mobile.BuildTools.Generators.Images;
using Xunit;
using Xunit.Abstractions;
using Mobile.BuildTools.Models.AppIcons;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Advanced;

namespace Mobile.BuildTools.Tests.Fixtures.Generators
{
    public class ImageResizer { }

    [CollectionDefinition(nameof(ImageResizer), DisableParallelization = true)]
    public class ImageResizerCollection : ICollectionFixture<ImageResizer> { }

    [Collection(nameof(ImageResizer))]
    public class ImageResizerGeneratorFixture : FixtureBase
    {
        public ImageResizerGeneratorFixture(ITestOutputHelper testOutputHelper) 
            : base(Path.Combine("Templates", "Apple"), testOutputHelper)
        {
        }

        [Theory]
        [InlineData("xxxhdpi", 1, 300)]
        [InlineData("xxhdpi", .75, 225)]
        [InlineData("xhdpi", .5, 150)]
        public void GeneratesImage(string resourcePath, double scale, int expectedOutput)
        {
            var config = GetConfiguration();
            config.IntermediateOutputPath += resourcePath;
            var generator = new ImageResizeGenerator(config);

            var image = new OutputImage
            {
                Height = 0,
                Width = 0,
                InputFile = Path.Combine(TestConstants.ImageDirectory, "dotnetbot.png"),
                OutputFile = Path.Combine(config.IntermediateOutputPath, "dotnetbot.png"),
                OutputLink = Path.Combine("Resources", "drawable-xxxhdpi", "dotnetbot.png"),
                RequiresBackgroundColor = false,
                Scale = scale,
                ShouldBeVisible = true,
                Watermark = null
            };

            var ex = Record.Exception(() => generator.ProcessImage(image));

            Assert.Null(ex);
            Assert.True(File.Exists(image.OutputFile));

            using var imageResource = Image.Load(image.OutputFile);
            Assert.Equal(expectedOutput, imageResource.Width);
        }

        [Theory]
        [InlineData("xxxhdpi", 300)]
        [InlineData("xxhdpi", 225)]
        [InlineData("xhdpi", 150)]
        public void GeneratesImageWithCustomHeightWidth(string resourcePath, int expectedOutput)
        {
            var config = GetConfiguration();
            config.IntermediateOutputPath += resourcePath;
            var generator = new ImageResizeGenerator(config);

            var image = new OutputImage
            {
                Height = expectedOutput,
                Width = expectedOutput,
                InputFile = Path.Combine(TestConstants.ImageDirectory, "dotnetbot.png"),
                OutputFile = Path.Combine(config.IntermediateOutputPath, "dotnetbot.png"),
                OutputLink = Path.Combine("Resources", "drawable-xxxhdpi", "dotnetbot.png"),
                RequiresBackgroundColor = false,
                Scale = 0,
                ShouldBeVisible = true,
                Watermark = null
            };

            var ex = Record.Exception(() => generator.ProcessImage(image));

            Assert.Null(ex);
            Assert.True(File.Exists(image.OutputFile));

            using var imageResource = Image.Load(image.OutputFile);
            Assert.Equal(expectedOutput, imageResource.Width);
        }

        [Theory]
        [InlineData("dotnetbot", "example")]
        [InlineData("dotnetbot", "beta-version")]
        [InlineData("icon", "example")]
        [InlineData("icon", "beta-version")]
        public void AppliesWatermark(string inputImageName, string watermarkImage)
        {
            var config = GetConfiguration();
            var generator = new ImageResizeGenerator(config);

            var image = new OutputImage
            {
                Height = 0,
                Width = 0,
                InputFile = Path.Combine(TestConstants.WatermarkImageDirectory, $"{inputImageName}.png"),
                OutputFile = Path.Combine($"{config.IntermediateOutputPath}-{inputImageName}-{watermarkImage}", $"{inputImageName}.png"),
                OutputLink = Path.Combine("Resources", "drawable-xxxhdpi", $"{inputImageName}.png"),
                RequiresBackgroundColor = false,
                Scale = 1,
                ShouldBeVisible = true,
                Watermark = new WatermarkConfiguration
                {
                    SourceFile = Path.Combine(TestConstants.WatermarkImageDirectory, $"{watermarkImage}.png")
                }
            };

            generator.ProcessImage(image);

            using var inputImage = Image.Load(image.InputFile);
            using var outputImage = Image.Load(image.OutputFile);
            using var inputClone = inputImage.CloneAs<Rgba32>();
            using var outputClone = outputImage.CloneAs<Rgba32>();

            bool appliedWatermark;
            for (var y = 0; y < inputImage.Height; ++y)
            {
                var inputPixelRowSpan = inputClone.GetPixelRowSpan(y);
                var outputPixelRowSpan = outputClone.GetPixelRowSpan(y);
                for (var x = 0; x < inputImage.Width; ++x)
                {
                    appliedWatermark = inputPixelRowSpan[x] == outputPixelRowSpan[x];
                    if (appliedWatermark)
                        return;
                }
            }

            _testOutputHelper.WriteLine("All pixels are the same in the Input and Output Images");
            Assert.True(false);
        }

        [Fact]
        public void SetsDefaultBackground()
        {
            var config = GetConfiguration();
            var generator = new ImageResizeGenerator(config);

            var image = new OutputImage
            {
                Height = 0,
                Width = 0,
                InputFile = Path.Combine(TestConstants.ImageDirectory, "dotnetbot.png"),
                OutputFile = Path.Combine(config.IntermediateOutputPath, "dotnetbot.png"),
                OutputLink = Path.Combine("Resources", "drawable-xxxhdpi", "dotnetbot.png"),
                RequiresBackgroundColor = true,
                Scale = 1,
                ShouldBeVisible = true,
                Watermark = null
            };

            generator.ProcessImage(image);

            using var inputImage = Image.Load(image.InputFile);
            using var outputImage = Image.Load(image.OutputFile);
            using var inputClone = inputImage.CloneAs<Rgba32>();
            using var outputClone = outputImage.CloneAs<Rgba32>();

            var comparedTransparentPixel = false;
            for (var y = 0; y < inputImage.Height; ++y)
            {
                var inputPixelRowSpan = inputClone.GetPixelRowSpan(y);
                var outputPixelRowSpan = outputClone.GetPixelRowSpan(y);
                for (var x = 0; x < inputImage.Width; ++x)
                {
                    var startingPixel = inputPixelRowSpan[x];
                    if (startingPixel.A == 0)
                    {
                        comparedTransparentPixel = true;
                        var pixel = outputPixelRowSpan[x];
                        Assert.Equal(255, pixel.R);
                        Assert.Equal(255, pixel.G);
                        Assert.Equal(255, pixel.B);
                        Assert.Equal(255, pixel.A);
                    }
                }
            }

            Assert.True(comparedTransparentPixel);
        }

        [Fact]
        public void SetsCustomBackground()
        {
            var config = GetConfiguration();
            var generator = new ImageResizeGenerator(config);

            var image = new OutputImage
            {
                Height = 0,
                Width = 0,
                InputFile = Path.Combine(TestConstants.ImageDirectory, "dotnetbot.png"),
                OutputFile = Path.Combine(config.IntermediateOutputPath, "dotnetbot.png"),
                OutputLink = Path.Combine("Resources", "drawable-xxxhdpi", "dotnetbot.png"),
                RequiresBackgroundColor = true,
                Scale = 1,
                ShouldBeVisible = true,
                Watermark = null,
                BackgroundColor = "#8A2BE2"
            };

            generator.ProcessImage(image);

            using var inputImage = Image.Load(image.InputFile);
            using var outputImage = Image.Load(image.OutputFile);
            using var inputClone = inputImage.CloneAs<Rgba32>();
            using var outputClone = outputImage.CloneAs<Rgba32>();

            var comparedTransparentPixel = false;
            for (var y = 0; y < inputImage.Height; ++y)
            {
                var inputPixelRowSpan = inputClone.GetPixelRowSpan(y);
                var outputPixelRowSpan = outputClone.GetPixelRowSpan(y);
                for (var x = 0; x < inputImage.Width; ++x)
                {
                    var startingPixel = inputPixelRowSpan[x];
                    if (startingPixel.A == 0)
                    {
                        comparedTransparentPixel = true;
                        var pixel = outputPixelRowSpan[x];
                        Assert.Equal(138, pixel.R);
                        Assert.Equal(43, pixel.G);
                        Assert.Equal(226, pixel.B);
                        Assert.Equal(255, pixel.A);
                    }
                }
            }

            Assert.True(comparedTransparentPixel);
        }

        [Theory]
        [InlineData("Dev")]
        [InlineData("Stage")]
        public void AppliesTextBanner(string text)
        {
            var config = GetConfiguration();
            var generator = new ImageResizeGenerator(config);

            var image = new OutputImage
            {
                Height = 0,
                Width = 0,
                InputFile = Path.Combine(TestConstants.ImageDirectory, "dotnetbot.png"),
                OutputFile = Path.Combine(config.IntermediateOutputPath, "dotnetbot.png"),
                OutputLink = Path.Combine("Resources", "drawable-xxxhdpi", "dotnetbot.png"),
                RequiresBackgroundColor = true,
                Scale = .5,
                ShouldBeVisible = true,
                Watermark = new WatermarkConfiguration
                {
                    Text = text
                }
            };

            generator.ProcessImage(image);
        }
    }
}
