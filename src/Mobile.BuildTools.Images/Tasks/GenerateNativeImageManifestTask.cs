using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Generators.Images;

namespace Mobile.BuildTools.Tasks
{
    /// <summary>Supplies only the manifest references that native resizetizers do not supply.</summary>
    public sealed class GenerateNativeImageManifestTask : Microsoft.Build.Utilities.Task
    {
        public ITaskItem[] Images { get; set; } = Array.Empty<ITaskItem>();
        public ITaskItem Manifest { get; set; }
        public ITaskItem[] PartialManifests { get; set; } = Array.Empty<ITaskItem>();
        public ITaskItem[] AppManifestEntries { get; set; } = Array.Empty<ITaskItem>();
        [Required] public string Platform { get; set; }
        [Required] public string ProjectDirectory { get; set; }
        [Required] public string OutputDirectory { get; set; }
        [Output] public ITaskItem GeneratedManifest { get; private set; }
        [Output] public ITaskItem[] FileWrites { get; private set; } = Array.Empty<ITaskItem>();

        public override bool Execute()
        {
            GeneratedManifest = null;
            FileWrites = Array.Empty<ITaskItem>();
            try
            {
                // Resizetizers process the first app icon. Images precede the icons mapped to Images.
                var icon = Images.FirstOrDefault(IsAppIcon);
                if (icon == null)
                    return true;
                var alias = icon.GetMetadata("Link");
                var name = Path.GetFileNameWithoutExtension(Normalize(string.IsNullOrWhiteSpace(alias) ? icon.ItemSpec : alias));
                if (string.IsNullOrEmpty(name))
                    throw new InvalidDataException("The native app icon has no output filename.");
                if (Platform.Equals("android", StringComparison.OrdinalIgnoreCase))
                    GenerateAndroid(name.ToLowerInvariant());
                else if (Platform.Equals("apple", StringComparison.OrdinalIgnoreCase))
                    GenerateApple(name);
            }
            catch (Exception exception)
            {
                Log.LogErrorFromException(exception, true);
            }
            return !Log.HasLoggedErrors;
        }

        private static bool IsAppIcon(ITaskItem image)
        {
            var type = image.GetMetadata("MobileBuildToolsItemType");
            return type == "MauiIcon" || type == "UnoIcon" ||
                ((type == "MauiImage" || type == "UnoImage") &&
                 string.Equals(image.GetMetadata("IsAppIcon"), "true", StringComparison.OrdinalIgnoreCase));
        }

        private void GenerateAndroid(string name)
        {
            if (Manifest == null)
                return;
            var source = Absolute(Manifest.ItemSpec);
            var document = ReadXml(source);
            if (document.Root?.Name.LocalName != "manifest")
                throw new InvalidDataException($"'{source}' is not an Android manifest.");
            XNamespace android = "http://schemas.android.com/apk/res/android";
            var application = document.Root.Element("application");
            if (application == null)
            {
                application = new XElement("application");
                document.Root.Add(application);
            }
            var expectedIcon = "@mipmap/" + name;
            var expectedRound = expectedIcon + "_round";
            var icon = (string)application.Attribute(android + "icon");
            var round = (string)application.Attribute(android + "roundIcon");
            var iconConflict = !string.IsNullOrWhiteSpace(icon) && icon != expectedIcon;
            var roundConflict = !string.IsNullOrWhiteSpace(round) && round != expectedRound;
            if (iconConflict)
                Conflict("android:icon", icon, expectedIcon);
            if (roundConflict)
                Conflict("android:roundIcon", round, expectedRound);
            // Keep an explicitly selected icon pair together instead of mixing a generated default
            // with a separately supplied icon. Users can intentionally select alternate resources.
            if (iconConflict || roundConflict)
                return;
            var changed = false;
            if (string.IsNullOrWhiteSpace(icon))
            {
                application.SetAttributeValue(android + "icon", expectedIcon);
                changed = true;
            }
            if (string.IsNullOrWhiteSpace(round))
            {
                application.SetAttributeValue(android + "roundIcon", expectedRound);
                changed = true;
            }
            if (!changed)
                return;
            if (document.Root.GetPrefixOfNamespace(android) == null)
                document.Root.SetAttributeValue(XNamespace.Xmlns + "android", android.NamespaceName);
            var identity = ImageFingerprint.Hash(source).Substring(0, 16);
            Write(document, Path.Combine(identity, "AndroidManifest.xml"), Manifest);
        }

        private void GenerateApple(string name)
        {
            var expected = "Assets.xcassets/" + name + ".appiconset";
            var manualIconKeys = new[] { "CFBundleIcons", "CFBundleIcons~ipad", "CFBundleIconFiles", "CFBundleIconFile", "CFBundleIconName" };
            var manualSelection = AppManifestEntries.Any(x => manualIconKeys.Contains(x.ItemSpec));
            var selections = AppManifestEntries.Where(x => x.ItemSpec == "XSAppIconAssets")
                .Select(x => x.GetMetadata("Value")).ToList();
            var inputs = (Manifest == null ? Array.Empty<ITaskItem>() : new[] { Manifest }).Concat(PartialManifests);
            foreach (var input in inputs)
            {
                var inputPath = Absolute(input.ItemSpec);
                using (var stream = File.OpenRead(inputPath))
                {
                    var header = new byte[8];
                    if (stream.Read(header, 0, header.Length) == header.Length && Encoding.ASCII.GetString(header) == "bplist00")
                    {
                        Log.LogWarning($"Cannot inspect the binary Apple manifest '{input.ItemSpec}' for an explicit icon selection. It is preserved; specify XSAppIconAssets in an XML plist or AppManifestEntry to select the generated icon '{expected}'.");
                        return;
                    }
                }
                var document = ReadXml(inputPath);
                var dictionary = document.Root?.Element("dict");
                if (dictionary == null)
                    throw new InvalidDataException($"'{input.ItemSpec}' is not an XML property-list manifest.");
                foreach (var key in dictionary.Elements("key"))
                {
                    if (key.Value == "XSAppIconAssets")
                        selections.Add(key.ElementsAfterSelf().FirstOrDefault()?.Value ?? string.Empty);
                    else if (manualIconKeys.Contains(key.Value))
                        manualSelection = true;
                    else if (key.Value == "XSAppIconAsset")
                        Log.LogWarning("The Apple manifest key 'XSAppIconAsset' is not read by the .NET Apple SDK. Use 'XSAppIconAssets' (plural).");
                }
            }
            foreach (var value in selections.Where(x => Normalize(x) != Normalize(expected)))
                Conflict("XSAppIconAssets", value, expected);
            if (selections.Count != 0)
                return;
            if (manualSelection)
            {
                Log.LogWarning($"The Apple manifest explicitly declares bundle icon keys. Those values are preserved; remove them to select the generated native app icon '{expected}' automatically.");
                return;
            }
            // actool owns CFBundleIcons/CFBundleIconFiles. Its selector belongs in the input
            // manifest, using the public, non-overwriting PartialAppManifest contract.
            var partial = new XDocument(new XDeclaration("1.0", "utf-8", null),
                new XElement("plist", new XAttribute("version", "1.0"),
                    new XElement("dict", new XElement("key", "XSAppIconAssets"), new XElement("string", expected))));
            Write(partial, "NativeImageInfo.plist", null);
            GeneratedManifest.SetMetadata("Overwrite", "false");
        }

        private void Conflict(string key, string actual, string expected) =>
            Log.LogWarning($"The explicit {key} reference '{actual}' differs from the generated native app icon '{expected}'. The explicit reference is preserved; verify that it is packaged, or remove it to use the generated icon.");

        private void Write(XDocument document, string relativePath, ITaskItem original)
        {
            var output = Absolute(Path.Combine(OutputDirectory, relativePath));
            using var stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = new UTF8Encoding(false), Indent = true, CloseOutput = false }))
                document.Save(writer);
            var bytes = stream.ToArray();
            if (!File.Exists(output) || !File.ReadAllBytes(output).SequenceEqual(bytes))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(output));
                File.WriteAllBytes(output, bytes);
            }
            GeneratedManifest = original == null ? new TaskItem(output) : new TaskItem(original) { ItemSpec = output };
            FileWrites = new[] { new TaskItem(output) };
        }

        private static XDocument ReadXml(string path)
        {
            // Apple XML plists may contain a DTD; never fetch it or expand external entities.
            using var reader = XmlReader.Create(path, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore, XmlResolver = null });
            return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
        }
        private string Absolute(string path) => Path.GetFullPath(Path.Combine(ProjectDirectory, Normalize(path)));
        private static string Normalize(string path) => path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
    }
}
