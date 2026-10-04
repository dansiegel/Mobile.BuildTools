using System.IO;
using E2E.Core;
using Xunit;

namespace E2E.Tests.Helpers;

internal static class ConsumerResources
{
    public static Stream Open(string name)
    {
        var resourceName = $"E2E.Core.PackageSmoke.{name}";
        var stream = typeof(CommonLib).Assembly.GetManifestResourceStream(resourceName);
        Assert.True(stream is not null, $"The built consumer is missing resource '{resourceName}'.");
        return stream;
    }

    public static string ReadText(string name)
    {
        using var stream = Open(name);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
