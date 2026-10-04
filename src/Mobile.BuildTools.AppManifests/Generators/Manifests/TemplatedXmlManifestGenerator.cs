using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Mobile.BuildTools.Build;

namespace Mobile.BuildTools.Generators.Manifests;

internal class TemplatedXmlManifestGenerator : BaseTemplatedManifestGenerator
{
    public TemplatedXmlManifestGenerator(IBuildConfiguration configuration) : base(configuration) { }

    internal static XDocument Parse(string manifest)
    {
        using var text = new StringReader(manifest);
        using var reader = XmlReader.Create(text, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore, XmlResolver = null });
        return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
    }

    protected override string TransformManifest(string manifest, IDictionary<string, string> variables)
    {
        var document = Parse(manifest);
        foreach (var attribute in document.Descendants().Attributes().Where(attribute => !attribute.IsNamespaceDeclaration))
            attribute.Value = ReplaceTokens(attribute.Value, variables);
        foreach (var text in document.DescendantNodes().OfType<XText>())
            text.Value = ReplaceTokens(text.Value, variables);
        Validate(document);
        return document.ToString(SaveOptions.DisableFormatting);
    }

    protected override bool IsFrameworkToken(string name) =>
        string.Equals(Path.GetExtension(ManifestInputPath), ".appxmanifest", System.StringComparison.OrdinalIgnoreCase) &&
        (name == "placeholder" || name == "targetnametoken" || name == "targetentrypoint");

    protected virtual void Validate(XDocument document) { }
    public override string GetBundId() => string.Empty;
    protected override string SetAppBundleId(string manifest, string packageName) => manifest;
}
