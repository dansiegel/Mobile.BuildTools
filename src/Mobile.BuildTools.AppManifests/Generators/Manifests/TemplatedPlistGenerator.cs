using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Mobile.BuildTools.Build;

namespace Mobile.BuildTools.Generators.Manifests;

internal class TemplatedPlistGenerator : TemplatedXmlManifestGenerator
{
    public TemplatedPlistGenerator(IBuildConfiguration configuration) : base(configuration) { }

    protected override void Validate(XDocument document)
    {
        foreach (var element in document.Descendants())
        {
            // Missing tokens remain intact when configured as warnings.
            if (GetMatches(element.Value).Count != 0)
                continue;
            var valid = element.Name.LocalName switch
            {
                "integer" => long.TryParse(element.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
                "real" => double.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) && !double.IsInfinity(number) && !double.IsNaN(number),
                "date" => DateTimeOffset.TryParse(element.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _),
                _ => true
            };
            if (!valid)
                throw new InvalidDataException($"Manifest contains an invalid plist {element.Name.LocalName} value. Replacement values are not logged.");
            if (element.Name.LocalName == "data")
            {
                try { Convert.FromBase64String(element.Value); }
                catch (FormatException) { throw new InvalidDataException("Manifest contains invalid plist base64 data. Replacement values are not logged."); }
            }
        }
    }

    internal static XElement FindValue(XDocument document, string key) =>
        document.Root?.Element("dict")?.Elements("key").FirstOrDefault(element => element.Value == key)?.ElementsAfterSelf().FirstOrDefault();

    internal static void SetValue(XDocument document, string key, string value)
    {
        var existing = FindValue(document, key);
        if (existing != null)
            existing.ReplaceWith(new XElement("string", value));
        else
            document.Root.Element("dict").Add(new XElement("key", key), new XElement("string", value));
    }

    public override string GetBundId() =>
        FindValue(Parse(File.ReadAllText(ManifestOutputPath)), "CFBundleIdentifier")?.Value ?? string.Empty;

    protected override string SetAppBundleId(string manifest, string packageName)
    {
        var document = Parse(manifest);
        SetValue(document, "CFBundleIdentifier", packageName);
        return document.ToString(SaveOptions.DisableFormatting);
    }
}
