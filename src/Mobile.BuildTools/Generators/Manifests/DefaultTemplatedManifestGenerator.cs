using System.IO;
using Mobile.BuildTools.Build;

namespace Mobile.BuildTools.Generators.Manifests
{
    internal class DefaultTemplatedManifestGenerator : BaseTemplatedManifestGenerator
    {
        public DefaultTemplatedManifestGenerator(IBuildConfiguration configuration)
            : base(configuration)
        {
        }

        protected override string ReadManifest() => File.ReadAllText(ManifestOutputPath);

        protected override void SaveManifest(string manifest) => File.WriteAllText(ManifestOutputPath, manifest);

        protected override string SetAppBundleId(string manifest, string packageName)
        {
            // Not Implemented
            Log.LogWarning($"An app bundle id '{packageName}' has been provided, but this may not be supported on this platform ({Build.Platform}).");
            return manifest;
        }
    }
}
