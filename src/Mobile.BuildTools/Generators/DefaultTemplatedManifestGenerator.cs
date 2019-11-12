using System.IO;

namespace Mobile.BuildTools.Generators
{
    internal class DefaultTemplatedManifestGenerator : BaseTemplatedManifestGenerator
    {
        protected override string ReadManifest() => File.ReadAllText(ManifestOutputPath);

        protected override void SaveManifest(string manifest) => File.WriteAllText(ManifestOutputPath, manifest);
    }
}
