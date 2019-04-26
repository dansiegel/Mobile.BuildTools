using System.IO;
using Xamarin.MacDev;

namespace Mobile.BuildTools.Generators
{
    public class TemplatedPlistGenerator : BaseTemplatedManifestGenerator
    {
        protected override string ReadManifest() => PDictionary.FromFile(ManifestOutputPath).ToXml();

        protected override void SaveManifest(string manifest)
        {
            var plist = PDictionary.FromString(manifest);
            File.WriteAllBytes(ManifestOutputPath, plist.ToByteArray(PropertyListFormat.Binary));
        }
    }
}
