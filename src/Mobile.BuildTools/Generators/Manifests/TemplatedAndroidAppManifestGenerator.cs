using System.Xml.Linq;
using Mobile.BuildTools.Build;
using Xamarin.Android.Tools;

namespace Mobile.BuildTools.Generators.Manifests
{
    internal class TemplatedAndroidAppManifestGenerator : BaseTemplatedManifestGenerator
    {
        public TemplatedAndroidAppManifestGenerator(IBuildConfiguration configuration)
            : base(configuration)
        {
            AndroidVersions = new AndroidVersions(AndroidVersions.KnownVersions);
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

        public override string GetBundId()
        {
            var manifest = AndroidAppManifest.Load(ManifestInputPath, AndroidVersions);
            return manifest.PackageName;
        }

        protected override string SetAppBundleId(string manifest, string packageName)
        {
            var appManifest = AndroidAppManifest.Load(XDocument.Parse(manifest), AndroidVersions);

            if(!string.IsNullOrEmpty(packageName))
                appManifest.PackageName = packageName;

            return appManifest.Document.ToString();
        }
    }
}
