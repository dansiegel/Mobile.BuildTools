using Mobile.BuildTools.Build;
using Xamarin.Android.Tools;

namespace Mobile.BuildTools.Generators.Versioning
{
    internal class AndroidAutomaticBuildVersionGenerator : BuildVersionGeneratorBase
    {
        public AndroidAutomaticBuildVersionGenerator(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        public string[] ReferenceAssemblyPaths { get; set; }

        protected override void ProcessManifest(string path, string buildNumber)
        {
            var androidManifest = AndroidAppManifest.Load(path, new AndroidVersions(ReferenceAssemblyPaths));
            androidManifest.VersionCode = buildNumber;
            androidManifest.VersionName = $"{SanitizeVersion(androidManifest.VersionName)}.{buildNumber}";
            androidManifest.WriteToFile(path);
        }
    }
}
