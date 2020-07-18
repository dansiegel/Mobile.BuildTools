using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mobile.BuildTools.Drawing;
using SixLabors.ImageSharp;
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
        public void GeneratesExpectedColor(string input, Color color)
        {
            bool success = false;
            Color result = default;
            var ex = Record.Exception(() => success = ColorUtils.TryParse(input, out result));
            Assert.Null(ex);
            Assert.True(success);
            Assert.Equal(color, result);
        }

        public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] { "Red", Color.Red },
            new object[] { "Blue", Color.Blue },
            new object[] { "OrangeRed", Color.OrangeRed },
            new object[] { "#FF88DD", Color.ParseHex("#FF88DD") },
        };
    }
}
