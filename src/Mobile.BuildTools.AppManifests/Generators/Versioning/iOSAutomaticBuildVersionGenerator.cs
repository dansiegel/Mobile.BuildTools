using System.IO;
using Mobile.BuildTools.Build;
using Xamarin.MacDev;

namespace Mobile.BuildTools.Generators.Versioning
{
    internal class iOSAutomaticBuildVersionGenerator : BuildVersionGeneratorBase
    {
        public iOSAutomaticBuildVersionGenerator(IBuildConfiguration buildConfiguration, string manifestPath, string outputPath)
            : base(buildConfiguration, manifestPath, outputPath)
        {
        }

        protected override void ProcessManifest(string plistPath, string outputPath, string buildNumber)
        {
            var infoPlist = PDictionary.FromFile(plistPath);
            infoPlist.SetCFBundleVersion(buildNumber);
            var shortVersion = $"{SanitizeVersion(infoPlist["CFBundleShortVersionString"] as PString)}.{buildNumber}";
            infoPlist.SetCFBundleShortVersionString(shortVersion);
            File.WriteAllBytes(outputPath, infoPlist.ToByteArray(PropertyListFormat.Xml));
        }
    }
}
