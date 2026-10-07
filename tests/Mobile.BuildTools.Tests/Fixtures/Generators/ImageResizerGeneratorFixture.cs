using System;
using System.IO;
using Mobile.BuildTools.Drawing;
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
        [InlineData("dotnetbot.png", 1.5)]
        [InlineData("dotnetbot.png", 1)]
        [InlineData("dotnetbot.png", .5)]
        [InlineData("dotnetbot.svg", 1.5)]
        [InlineData("dotnetbot.svg", 1)]
        [InlineData("dotnetbot.svg", .5)]
        [InlineData("logo.svg", 1.5)]
        [InlineData("logo.svg", 1)]
        public void AppliesPadding(string inputFile, double paddingFactor)
        {
            var config = GetConfiguration();
            config.IntermediateOutputPath += GetOutputDirectorySuffix((nameof(paddingFactor), paddingFactor), (nameof(inputFile), inputFile));
            var generator = new ImageResizeGenerator(config);

            var outputFilename = $"{Path.GetFileNameWithoutExtension(inputFile)}.png";

            var image = new OutputImage
            {
                Height = 0,
                Width = 0,
                InputFile = Path.Combine(TestConstants.ImageDirectory, inputFile),
                OutputFile = Path.Combine(config.IntermediateOutputPath, outputFilename),
                OutputLink = Path.Combine("Resources", "drawable-xxxhdpi", outputFilename),
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
            Assert.True(File.Exists(image.OutputFile));
            using var source = ImageBase.Load(image.InputFile);
            using var output = SKBitmap.Decode(image.OutputFile);
            Assert.NotNull(output);
            var size = source.GetOriginalSize();
            var width = image.Width;
            var height = image.Height;
            if (width > 0 && height == 0)
                height = Math.Max(1, (int)Math.Round((double)size.Height * width / size.Width));
            else if (height > 0 && width == 0)
                width = Math.Max(1, (int)Math.Round((double)size.Width * height / size.Height));
            else if (width == 0 && height == 0)
            {
                var scale = image.Scale > 0 ? image.Scale : 1;
                width = Math.Max(1, (int)Math.Round(size.Width * scale));
                height = Math.Max(1, (int)Math.Round(size.Height * scale));
            }
            Assert.Equal(width, output.Width);
            Assert.Equal(height, output.Height);
            if (image.RequiresBackgroundColor)
                Assert.False(output.HasTransparentBackground());
        }
    }
}
