using System.IO;
using Mobile.BuildTools.Build;
using Xamarin.MacDev;

namespace Mobile.BuildTools.Generators.Versioning
{
    internal class iOSAutomaticBuildVersionGenerator : BuildVersionGeneratorBase
    {
        public iOSAutomaticBuildVersionGenerator(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        protected override void ProcessManifest(string plistPath, string buildNumber)
        {
            var infoPlist = PDictionary.FromFile(plistPath);
            infoPlist.SetCFBundleVersion(buildNumber);
            var shortVersion = $"{SanitizeVersion(infoPlist["CFBundleShortVersionString"] as PString)}.{buildNumber}";
            infoPlist.SetCFBundleShortVersionString(shortVersion);
            File.WriteAllBytes(plistPath, infoPlist.ToByteArray(PropertyListFormat.Binary));
        }
    }
}
