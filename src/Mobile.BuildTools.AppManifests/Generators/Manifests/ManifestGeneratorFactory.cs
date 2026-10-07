using System.IO;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Generators.Manifests;

internal static class ManifestGeneratorFactory
{
    internal static BaseTemplatedManifestGenerator Create(string path, IBuildConfiguration config) =>
        Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".plist" => new TemplatedPlistGenerator(config),
            ".json" or ".webmanifest" => new DefaultTemplatedManifestGenerator(config),
            ".xml" when config.Platform == Platform.Android => new TemplatedAndroidAppManifestGenerator(config),
            ".xml" or ".appxmanifest" => new TemplatedXmlManifestGenerator(config),
            _ => null
        };
}
