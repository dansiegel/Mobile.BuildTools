using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mobile.BuildTools.Models.AppIcons;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures.Models
{
    public class OutputImageFixture : FixtureBase
    {
        public OutputImageFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void IgnoresCustomSettingsWithSourceFile()
        {
            var outputImage = new OutputImage
            {
                OutputFile = "Sample.png",
                Watermark = new WatermarkConfiguration
                {
                    Colors = new[] { "Foo", "Bar" },
                    FontFamily = "MakeBelieve",
                    Text = "Hello World",
                    SourceFile = "FooBar.png",
                    Opacity = 0.5
                }
            };

            var taskItem = outputImage.ToTaskItem();
            var convertedImage = taskItem.ToOutputImage();

            Assert.NotNull(convertedImage.Watermark);
            Assert.Null(convertedImage.Watermark.Colors);
            Assert.Null(convertedImage.Watermark.Text);
            Assert.Null(convertedImage.Watermark.FontFamily);

            Assert.Equal(outputImage.Watermark.SourceFile, convertedImage.Watermark.SourceFile);
            Assert.Equal(outputImage.Watermark.Opacity, convertedImage.Watermark.Opacity);
        }

        [Fact]
        public void ConvertedImageUsesDefaultValues()
        {
            var outputImage = new OutputImage
            {
                OutputFile = "Sample.png",
                Watermark = new WatermarkConfiguration
                {
                    Text = "Hello World",
                }
            };

            var taskItem = outputImage.ToTaskItem();
            var convertedImage = taskItem.ToOutputImage();

            Assert.NotNull(convertedImage.Watermark);
            Assert.Equal(outputImage.Watermark.Text, convertedImage.Watermark.Text);

            Assert.Equal("White", convertedImage.Watermark.TextColor);
            Assert.Equal(new[] { "Red", "OrangeRed" }, convertedImage.Watermark.Colors);
            Assert.Equal("Arial", convertedImage.Watermark.FontFamily);
            Assert.Equal(WatermarkPosition.BottomRight, convertedImage.Watermark.Position);
            Assert.Equal(Constants.DefaultOpacity, convertedImage.Watermark.Opacity);
        }
    }
}
