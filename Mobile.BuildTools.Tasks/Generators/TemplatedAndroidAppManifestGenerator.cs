using System.Xml.Linq;
using Xamarin.Android.Tools;

namespace Mobile.BuildTools.Generators
{
    public class TemplatedAndroidAppManifestGenerator : BaseTemplatedManifestGenerator
    {
        public TemplatedAndroidAppManifestGenerator(string[] referenceAssemblyPaths)
        {
            AndroidVersions = new AndroidVersions(referenceAssemblyPaths);
        }

        public AndroidVersions AndroidVersions { get; set; }

        protected override string ReadManifest() => 
            AndroidAppManifest.Load(ManifestOutputPath, AndroidVersions)
                              .Document
                              .ToString();

        protected override void SaveManifest(string manifest)
        {
            var doc = XDocument.Parse(manifest);
            AndroidAppManifest.Load(doc, AndroidVersions).WriteToFile(ManifestOutputPath);
        }
    }
}
