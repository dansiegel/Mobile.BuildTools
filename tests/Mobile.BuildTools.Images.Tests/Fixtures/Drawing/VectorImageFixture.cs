using System.IO;
using Mobile.BuildTools.Drawing;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures.Drawing
{
    public class VectorImageFixture : FixtureBase
    {
        public VectorImageFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData("dotnetbot.svg", true)]
        [InlineData("logo.svg", true)]
        [InlineData("square.svg", false)]
        public void DetectsTransparency(string inputFile, bool hasTransparentBackground)
        {
            using var vectorImage = new VectorImage(Path.Combine(TestConstants.ImageDirectory, inputFile));

            Assert.Equal(hasTransparentBackground, vectorImage.HasTransparentBackground);
        }
    }
}
