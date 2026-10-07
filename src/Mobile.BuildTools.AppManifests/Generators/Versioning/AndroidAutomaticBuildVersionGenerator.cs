using Mobile.BuildTools.Build;
using System.IO;
using System.Xml.Linq;
using Mobile.BuildTools.Generators.Manifests;

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
            var document = TemplatedXmlManifestGenerator.Parse(File.ReadAllText(path));
            XNamespace android = "http://schemas.android.com/apk/res/android";
            var version = document.Root.Attribute(android + "versionName")?.Value;
            document.Root.SetAttributeValue(android + "versionCode", buildNumber);
            document.Root.SetAttributeValue(android + "versionName", $"{SanitizeVersion(version)}.{buildNumber}");
            WriteIfChanged(outputPath, document.ToString(SaveOptions.DisableFormatting));
        }
    }
}
