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
    }
}
