using System.IO;
using System.Xml.Linq;
using Mobile.BuildTools.Build;

namespace Mobile.BuildTools.Generators.Manifests;

internal class TemplatedAndroidAppManifestGenerator : TemplatedXmlManifestGenerator
{
    public TemplatedAndroidAppManifestGenerator(IBuildConfiguration configuration) : base(configuration) { }

    public override string GetBundId() =>
        Parse(File.ReadAllText(ManifestOutputPath)).Root?.Attribute("package")?.Value ?? string.Empty;

    protected override string SetAppBundleId(string manifest, string packageName)
    {
        var document = Parse(manifest);
        if (!string.IsNullOrEmpty(packageName))
            document.Root.SetAttributeValue("package", packageName);
        return document.ToString(SaveOptions.DisableFormatting);
    }
}
