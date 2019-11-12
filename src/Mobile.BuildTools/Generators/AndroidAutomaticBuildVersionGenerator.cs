using Xamarin.Android.Tools;

namespace Mobile.BuildTools.Generators
{
    public class AndroidAutomaticBuildVersionGenerator : BuildVersionGeneratorBase
    {
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
