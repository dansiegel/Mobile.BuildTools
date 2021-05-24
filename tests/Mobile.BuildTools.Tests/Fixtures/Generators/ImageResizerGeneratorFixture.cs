using System.IO;
using System.Text;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Models.AppIcons;
using SkiaSharp;
using Xunit;
using Xunit.Abstractions;

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
        [InlineData("dotnetbot.png", "xxxhdpi", 1)]
        [InlineData("dotnetbot.png", "xxhdpi", .75)]
        [InlineData("dotnetbot.png", "xhdpi", .5)]
        [InlineData("dotnetbot.svg", "xxxhdpi", 1)]
        [InlineData("dotnetbot.svg", "xxhdpi", .75)]
        [InlineData("dotnetbot.svg", "xhdpi", .5)]
        public void GeneratesImage(string inputFile, string resourcePath, double scale)
        {
            var config = GetConfiguration();
            config.IntermediateOutputPath += GetOutputDirectorySuffix((nameof(inputFile), inputFile), (nameof(scale), scale));
            var generator = new ImageResizeGenerator(config);

            var image = new OutputImage
            {
                Height = 0,
                Width = 0,
                InputFile = Path.Combine(TestConstants.ImageDirectory, inputFile),
                OutputFile = Path.Combine(config.IntermediateOutputPath, "dotnetbot.png"),
                OutputLink = Path.Combine("Resources", resourcePath, "dotnetbot.png"),
                RequiresBackgroundColor = false,
                Scale = scale,
                ShouldBeVisible = true,
                Watermark = null
            };

            var ex = Record.Exception(() => generator.ProcessImage(image));

            Assert.Null(ex);

            VerifyImageContents(image);
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
            config.IntermediateOutputPath += GetOutputDirectorySuffix((nameof(inputFile), inputFile), (nameof(expectedOutput), expectedOutput));
            var generator = new ImageResizeGenerator(config);

            var image = new OutputImage
            {
                Height = expectedOutput,
                Width = expectedOutput,
                InputFile = Path.Combine(TestConstants.ImageDirectory, inputFile),
                OutputFile = Path.Combine(config.IntermediateOutputPath, "dotnetbot.png"),
                OutputLink = Path.Combine("Resources", resourcePath, "dotnetbot.png"),
                RequiresBackgroundColor = false,
                Scale = 0,
                ShouldBeVisible = true,
                Watermark = null
            };

            var ex = Record.Exception(() => generator.ProcessImage(image));

            Assert.Null(ex);

            VerifyImageContents(image);
        }

        [Theory]
        [InlineData("dotnetbot", "example")]
        [InlineData("dotnetbot", "beta-version")]
        [InlineData("icon", "example")]
        [InlineData("icon", "beta-version")]
        public void AppliesWatermark(string inputImageName, string watermarkImage)
        {
            var config = GetConfiguration();
            config.IntermediateOutputPath += GetOutputDirectorySuffix((nameof(inputImageName), inputImageName), (nameof(watermarkImage), watermarkImage));
            var generator = new ImageResizeGenerator(config);

            var image = new OutputImage
            {
                Height = 0,
                Width = 0,
                InputFile = Path.Combine(TestConstants.WatermarkImageDirectory, $"{inputImageName}.png"),
                OutputFile = Path.Combine(config.IntermediateOutputPath, $"{inputImageName}.png"),
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

            VerifyImageContents(image);
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

            VerifyImageContents(image);
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

            VerifyImageContents(image);
        }

        [Theory]
        [InlineData("Dev", 0.5, WatermarkPosition.BottomLeft)]
        [InlineData("Dev", 1.0, WatermarkPosition.BottomLeft)]
        [InlineData("Stage", 0.5, WatermarkPosition.BottomLeft)]
        [InlineData("Something long", 1.0, WatermarkPosition.BottomLeft)]
        [InlineData("Dev", 0.5, WatermarkPosition.BottomRight)]
        [InlineData("Dev", 1.0, WatermarkPosition.BottomRight)]
        [InlineData("Stage", 0.5, WatermarkPosition.BottomRight)]
        [InlineData("Something long", 1.0, WatermarkPosition.BottomRight)]
        [InlineData("Dev", 0.5, WatermarkPosition.Bottom)]
        [InlineData("Dev", 1.0, WatermarkPosition.Bottom)]
        [InlineData("Stage", 0.5, WatermarkPosition.Bottom)]
        [InlineData("Something long", 1.0, WatermarkPosition.Bottom)]
        [InlineData("Dev", 0.5, WatermarkPosition.TopLeft)]
        [InlineData("Dev", 1.0, WatermarkPosition.TopLeft)]
        [InlineData("Stage", 0.5, WatermarkPosition.TopLeft)]
        [InlineData("Something long", 1.0, WatermarkPosition.TopLeft)]
        [InlineData("Dev", 0.5, WatermarkPosition.TopRight)]
        [InlineData("Dev", 1.0, WatermarkPosition.TopRight)]
        [InlineData("Stage", 0.5, WatermarkPosition.TopRight)]
        [InlineData("Something long", 1.0, WatermarkPosition.TopRight)]
        [InlineData("Dev", 0.5, WatermarkPosition.Top)]
        [InlineData("Dev", 1.0, WatermarkPosition.Top)]
        [InlineData("Stage", 0.5, WatermarkPosition.Top)]
        [InlineData("Something long", 1.0, WatermarkPosition.Top)]
        public void AppliesTextBanner(string text, double scale, WatermarkPosition position)
        {
            var config = GetConfiguration();
            config.IntermediateOutputPath += GetOutputDirectorySuffix((nameof(text), text), (nameof(scale), scale), (nameof(position), position));
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
                    Text = text,
                    Position = position
                }
            };

            generator.ProcessImage(image);

            VerifyImageContents(image);
        }

        [Theory]
        [InlineData("dotnetbot.png", 1)]
        [InlineData("dotnetbot.png", .75)]
        [InlineData("dotnetbot.png", .5)]
        [InlineData("dotnetbot.svg", 1)]
        [InlineData("dotnetbot.svg", .75)]
        [InlineData("dotnetbot.svg", .5)]
        public void AppliesPadding(string inputFile, double paddingFactor)
        {
            var config = GetConfiguration();
            config.IntermediateOutputPath += GetOutputDirectorySuffix((nameof(paddingFactor), paddingFactor), (nameof(inputFile), inputFile));
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

            VerifyImageContents(image);
        }

        private static string GetOutputDirectorySuffix(params (string, object)[] values)
        {
            var builder = new StringBuilder();

            foreach (var value in values)
            {
                var prefix = "and";
                if (builder.Length == 0)
                {
                    prefix = "-with";
                }

                builder.Append($"-{prefix}-{value.Item1}-of-{value.Item2}");
            }

            return builder.ToString();
        }

        private void VerifyImageContents(OutputImage image)
        {
            var expectedFilePath = Path.Combine(TestConstants.ExpectedImageDirectory, image.OutputFile);
            var outputFilePath = image.OutputFile;

            Assert.True(File.Exists(expectedFilePath), $"Expected image file '{expectedFilePath}' does not exist");
            Assert.True(File.Exists(outputFilePath), $"Resulting image file '{outputFilePath}' does not exist");

            using var expectedImage = SKBitmap.Decode(expectedFilePath);
            using var outputImage = SKBitmap.Decode(outputFilePath);

            Assert.Equal(expectedImage.Width, outputImage.Width);
            Assert.Equal(expectedImage.Height, outputImage.Height);

            for (var y = 0; y < expectedImage.Height; ++y)
            {
                for (var x = 0; x < expectedImage.Width; ++x)
                {
                    Assert.Equal(expectedImage.GetPixel(x, y), outputImage.GetPixel(x, y));
                }
            }
        }
    }
}
