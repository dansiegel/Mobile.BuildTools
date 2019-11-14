using System.IO;
using Mobile.BuildTools.Build;
using Xamarin.MacDev;

namespace Mobile.BuildTools.Generators.Manifests
{
    internal class TemplatedPlistGenerator : BaseTemplatedManifestGenerator
    {
        public TemplatedPlistGenerator(IBuildConfiguration configuration)
            : base(configuration)
        {
        }

        protected override string ReadManifest() => PDictionary.FromFile(ManifestOutputPath).ToXml();

        protected override void SaveManifest(string manifest)
        {
            var plist = PDictionary.FromString(manifest);
            File.WriteAllBytes(ManifestOutputPath, plist.ToByteArray(PropertyListFormat.Binary));
        }
    }
}
