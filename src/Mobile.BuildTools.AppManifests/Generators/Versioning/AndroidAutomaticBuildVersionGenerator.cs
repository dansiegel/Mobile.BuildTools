using Mobile.BuildTools.Build;
using Xamarin.Android.Tools;

namespace Mobile.BuildTools.Generators.Versioning
{
    internal class AndroidAutomaticBuildVersionGenerator : BuildVersionGeneratorBase
    {
        public AndroidAutomaticBuildVersionGenerator(IBuildConfiguration buildConfiguration, string manifestPath, string outputPath)
            : base(buildConfiguration, manifestPath, outputPath)
        {
        }

        protected override void ProcessManifest(string path, string outputPath, string buildNumber)
        {
            var androidManifest = AndroidAppManifest.Load(path, new AndroidVersions(AndroidVersions.KnownVersions));
            androidManifest.VersionCode = buildNumber;
            androidManifest.VersionName = $"{SanitizeVersion(androidManifest.VersionName)}.{buildNumber}";
            androidManifest.WriteToFile(outputPath);
        }
    }
}
