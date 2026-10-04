using Mobile.BuildTools.Utils;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Extensions;
public class PlatformExtensionsFixture
{
    [Theory]
    [InlineData("net8.0", Platform.Unsupported)]
    [InlineData("net8.0-ios", Platform.iOS)]
    [InlineData("net8.0-ios14.0", Platform.iOS)]
    [InlineData("net8.0-android", Platform.Android)]
    [InlineData("net8.0-android34", Platform.Android)]
    [InlineData("net8.0-android34.0", Platform.Android)]
    [InlineData("net10.0-android36.0", Platform.Android)]
    [InlineData("net10.0-ios26.0", Platform.iOS)]
    [InlineData("net10.0-maccatalyst26.0", Platform.macOS)]
    [InlineData("net10.0-windows10.0.19041.0", Platform.Windows)]
    [InlineData("net10.0-browser", Platform.WebAssembly)]
    [InlineData("net11.0-browser", Platform.WebAssembly)]
    [InlineData("net10.0-tvos26.0", Platform.TVOS)]
    [InlineData(null, Platform.Unsupported)]
    [InlineData("", Platform.Unsupported)]
    public void GetTargetPlatform(string framework, Platform expected)
    {
        var result = framework.GetTargetPlatform();
        Assert.Equal(expected, result);
    }
}
