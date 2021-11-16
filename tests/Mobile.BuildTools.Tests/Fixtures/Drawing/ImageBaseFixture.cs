using System.IO;
using Mobile.BuildTools.Drawing;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures.Drawing
{
    public class ImageBaseFixture : FixtureBase
    {
        public ImageBaseFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void LoadShouldThrowIfFileDoesNotExist() => 
            Assert.Throws<FileNotFoundException>(() => ImageBase.Load($"{Path.GetRandomFileName()}.svg"));
    }
}
