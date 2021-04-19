using System.IO;
using Mobile.BuildTools.Generators.Images;
using Xunit;
using Xunit.Abstractions;
using Mobile.BuildTools.Models.AppIcons;
using Mobile.BuildTools.Drawing;
using SkiaSharp;

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
        [InlineData("dotnetbot.png", "xxxhdpi", 1, 300)]
        [InlineData("dotnetbot.png", "xxhdpi", .75, 225)]
        [InlineData("dotnetbot.png", "xhdpi", .5, 150)]
        [InlineData("dotnetbot.svg", "xxxhdpi", 1, 419)]
        [InlineData("dotnetbot.svg", "xxhdpi", .75, 314)]
        [InlineData("dotnetbot.svg", "xhdpi", .5, 209)]
        public void GeneratesImage(string inputFile, string resourcePath, double scale, int expectedOutput)
        {
            var config = GetConfiguration();
            config.IntermediateOutputPath += resourcePath;
            config.IntermediateOutputPath += $"-with-{inputFile}";
            var generator = new ImageResizeGenerator(config);

            var image = new OutputImage
            {
                Height = 0,
                Width = 0,
                InputFile = Path.Combine(TestConstants.ImageDirectory, inputFile),
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

            using var imageResource = ImageBase.Load(image.OutputFile);
            Assert.Equal(expectedOutput, imageResource.Width);
        }

        [Theory]
        [InlineData("dotnetbot.png", "xxxhdpi", 300)]
        [InlineData("dotnetbot.png", "xxhdpi", 225)]
        [InlineData("dotnetbot.png", "xhdpi", 150)]
        [InlineData("dotnetbot.svg", "xxxhdpi", 300)]
        [InlineData("dotnetbot.svg", "xxhdpi", 225)]
        [InlineData("dotnetbot.svg", "xhdpi", 150)]
        public void GeneratesImageWithCustomHeightWidth(string inputFile, string resourcePath, int expectedOutput)
        {
            var config = GetConfiguration();
            config.IntermediateOutputPath += resourcePath;
            config.IntermediateOutputPath += $"-with-{inputFile}";
            var generator = new ImageResizeGenerator(config);

            var image = new OutputImage
            {
                Height = expectedOutput,
                Width = expectedOutput,
                InputFile = Path.Combine(TestConstants.ImageDirectory, inputFile),
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

            using var imageResource = ImageBase.Load(image.OutputFile);
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

            //using var inputImage = Image.Load(image.InputFile);
            //using var outputImage = Image.Load(image.OutputFile);
            //using var inputClone = inputImage.CloneAs<Rgba32>();
            //using var outputClone = outputImage.CloneAs<Rgba32>();

            //bool appliedWatermark;
            //for (var y = 0; y < inputImage.Height; ++y)
            //{
            //    var inputPixelRowSpan = inputClone.GetPixelRowSpan(y);
            //    var outputPixelRowSpan = outputClone.GetPixelRowSpan(y);
            //    for (var x = 0; x < inputImage.Width; ++x)
            //    {
            //        appliedWatermark = inputPixelRowSpan[x] == outputPixelRowSpan[x];
            //        if (appliedWatermark)
            //            return;
            //    }
            //}

            //_testOutputHelper.WriteLine("All pixels are the same in the Input and Output Images");
            //Assert.True(false);
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

            using var inputImage = ImageBase.Load(image.InputFile);
            using var outputImage = ImageBase.Load(image.OutputFile);

            Assert.True(inputImage.HasTransparentBackground);
            Assert.False(outputImage.HasTransparentBackground);
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

            using var inputImage = SKBitmap.Decode(image.InputFile);
            using var outputImage = SKBitmap.Decode(image.OutputFile);

            var comparedTransparentPixel = false;
            for (var y = 0; y < inputImage.Height; ++y)
            {
                for (var x = 0; x < inputImage.Width; ++x)
                {
                    if (inputImage.GetPixel(x, y).Alpha == 0)
                    {
                        comparedTransparentPixel = true;

                        var pixel = outputImage.GetPixel(x, y);

                        Assert.Equal(138, pixel.Red);
                        Assert.Equal(43, pixel.Green);
                        Assert.Equal(226, pixel.Blue);
                        Assert.Equal(255, pixel.Alpha);
                    }
                }
            }

            Assert.True(comparedTransparentPixel);
        }

        [Theory]
        [InlineData("Dev", 0.5)]
        [InlineData("Dev", 1.0)]
        [InlineData("Stage", 0.5)]
        [InlineData("Something long", 0.5)]
        public void AppliesTextBanner(string text, double scale)
        {
            var config = GetConfiguration();
            config.IntermediateOutputPath += text;
            config.IntermediateOutputPath += scale.ToString();
            var generator = new ImageResizeGenerator(config);

            var image = new OutputImage
            {
                Height = 0,
                Width = 0,
                InputFile = Path.Combine(TestConstants.ImageDirectory, "dotnetbot.png"),
                OutputFile = Path.Combine(config.IntermediateOutputPath, "dotnetbot.png"),
                OutputLink = Path.Combine("Resources", "drawable-xxxhdpi", "dotnetbot.png"),
                RequiresBackgroundColor = true,
                Scale = scale,
                ShouldBeVisible = true,
                Watermark = new WatermarkConfiguration
                {
                    Text = text
                }
            };

            generator.ProcessImage(image);
        }

        [Theory]
        [InlineData("dotnetbot.png", "nopadding", 1, 300)]
        [InlineData("dotnetbot.png", "25% pad", .75, 300)]
        [InlineData("dotnetbot.png", "50% pad", .5, 300)]
        [InlineData("dotnetbot.svg", "nopadding", 1, 419)]
        [InlineData("dotnetbot.svg", "25% pad", .75, 419)]
        [InlineData("dotnetbot.svg", "50% pad", .5, 419)]
        public void AppliesPadding(string inputFile, string resourcePath, double paddingFactor, int expectedOutput)
        {
            var config = GetConfiguration();
            config.IntermediateOutputPath += resourcePath;
            config.IntermediateOutputPath += $"-with-{inputFile}";
            var generator = new ImageResizeGenerator(config);

            var image = new OutputImage
            {
                Height = 0,
                Width = 0,
                InputFile = Path.Combine(TestConstants.ImageDirectory, inputFile),
                OutputFile = Path.Combine(config.IntermediateOutputPath, "dotnetbot.png"),
                OutputLink = Path.Combine("Resources", "drawable-xxxhdpi", "dotnetbot.png"),
                RequiresBackgroundColor = false,
                Scale = 1.0,
                ShouldBeVisible = true,
                Watermark = null,
                BackgroundColor = "Red",
                PaddingColor = "Yellow",
                PaddingFactor = paddingFactor
            };

            var ex = Record.Exception(() => generator.ProcessImage(image));

            Assert.Null(ex);
            Assert.True(File.Exists(image.OutputFile));

            using var imageResource = ImageBase.Load(image.OutputFile);
            Assert.Equal(expectedOutput, imageResource.Width);
        }
    }
}
