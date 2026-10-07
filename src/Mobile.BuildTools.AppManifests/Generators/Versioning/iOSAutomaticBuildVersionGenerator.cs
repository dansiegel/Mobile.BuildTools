using System.IO;
using Mobile.BuildTools.Build;
using System.Xml.Linq;
using Mobile.BuildTools.Generators.Manifests;

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
            var document = TemplatedXmlManifestGenerator.Parse(File.ReadAllText(plistPath));
            var version = TemplatedPlistGenerator.FindValue(document, "CFBundleShortVersionString")?.Value;
            TemplatedPlistGenerator.SetValue(document, "CFBundleVersion", buildNumber);
            TemplatedPlistGenerator.SetValue(document, "CFBundleShortVersionString", $"{SanitizeVersion(version)}.{buildNumber}");
            WriteIfChanged(outputPath, document.ToString(SaveOptions.DisableFormatting));
        }
    }
}
