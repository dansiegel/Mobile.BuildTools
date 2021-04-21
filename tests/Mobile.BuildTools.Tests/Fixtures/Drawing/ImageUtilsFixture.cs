using System.Collections.Generic;
using Mobile.BuildTools.Drawing;
using SkiaSharp;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures.Drawing
{
    public class ImageUtilsFixture : FixtureBase
    {
        public ImageUtilsFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void GeneratesExpectedColor(string input, SKColor color)
        {
            bool success = false;
            SKColor result = default;
            var ex = Record.Exception(() => success = ColorUtils.TryParse(input, out result));
            Assert.Null(ex);
            Assert.True(success);
            Assert.Equal(color, result);
        }

        public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] { "Red", SKColors.Red },
            new object[] { "red", SKColors.Red },
            new object[] { "Blue", SKColors.Blue },
            new object[] { "blue", SKColors.Blue },
            new object[] { "OrangeRed", SKColors.OrangeRed },
            new object[] { "orangered", SKColors.OrangeRed },
            new object[] { "#FF88DD", SKColor.Parse("#FF88DD") },
        };
    }
}
