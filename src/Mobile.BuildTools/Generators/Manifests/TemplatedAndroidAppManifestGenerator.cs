using System.Xml.Linq;
using Mobile.BuildTools.Build;
using Xamarin.Android.Tools;

namespace Mobile.BuildTools.Generators.Manifests
{
    internal class TemplatedAndroidAppManifestGenerator : BaseTemplatedManifestGenerator
    {
        public TemplatedAndroidAppManifestGenerator(IBuildConfiguration configuration, string[] referenceAssemblyPaths)
            : base(configuration)
        {
            AndroidVersions = new AndroidVersions(referenceAssemblyPaths);
        }

        public AndroidVersions AndroidVersions { get; set; }

        protected override string ReadManifest() => 
            AndroidAppManifest.Load(ManifestInputPath, AndroidVersions)
                              .Document
                              .ToString();

        protected override void SaveManifest(string manifest)
        {
            var doc = XDocument.Parse(manifest);
            AndroidAppManifest.Load(doc, AndroidVersions).WriteToFile(ManifestOutputPath);
        }

        protected override string SetAppBundleId(string manifest, string packageName)
        {
            var appManifest = AndroidAppManifest.Load(XDocument.Parse(manifest), AndroidVersions);

            if(!string.IsNullOrEmpty(packageName))
                appManifest.PackageName = packageName;

            PackageId = appManifest.PackageName;

            return appManifest.Document.ToString();
        }
    }
}
