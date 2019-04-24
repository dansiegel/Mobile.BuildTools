using System.IO;
using Xamarin.MacDev;

namespace Mobile.BuildTools.Generators
{
    public class iOSAutomaticBuildVersionGenerator : BuildVersionGeneratorBase
    {
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
