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
    public void GetTargetPlatform(string framework, Platform expected)
    {
        var result = framework.GetTargetPlatform();
        Assert.Equal(expected, result);
    }
}
