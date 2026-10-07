using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;

namespace Mobile.BuildTools.Generators.Images
{
    /// <summary>Offline raw assets, independent of the raster/vector image pipeline.</summary>
    internal sealed class LottieAssetPreparer
    {
        private readonly IBuildConfiguration build;
        private readonly string root;
        private readonly string[] searchFolders;
        private readonly string configurationFile;
        private readonly List<string> fileWrites = new List<string>();
        private sealed class Output
        {
            public string Source;
            public string LogicalName;
            public byte[] Bytes;
            public ITaskItem Item;
        }

        public LottieAssetPreparer(IBuildConfiguration build, string targetFramework, string runtimeIdentifier,
            IEnumerable<string> searchFolders, string configurationFile)
        {
            this.build = build;
            this.searchFolders = searchFolders.Where(Directory.Exists).Select(Path.GetFullPath).Distinct().ToArray();
            this.configurationFile = File.Exists(configurationFile) ? configurationFile : null;
            // Hash complete values rather than sanitized segments, which can collide.
            root = Path.Combine(Absolute(build.IntermediateOutputPath), "Mobile.BuildTools", "lottie",
                ImageFingerprint.Hash(Path.GetFullPath(build.ProjectDirectory) + "\n" + build.BuildConfiguration + "\n" + targetFramework + "\n" + runtimeIdentifier));
        }

        public IReadOnlyList<string> FileWrites => fileWrites;

        public ITaskItem[] Prepare(IEnumerable<ITaskItem> items)
        {
            var outputs = new Dictionary<string, Output>(StringComparer.OrdinalIgnoreCase);
            var optimize = build.Configuration?.Images?.OptimizeLottie ?? true;
            var declarations = items.ToArray();
            var manifest = Path.Combine(root, "outputs.json");
            var stamp = Path.Combine(root, "inputs.txt");
            if (declarations.Length == 0 && !File.Exists(manifest)) return Array.Empty<ITaskItem>();
            foreach (var item in declarations)
            {
                var original = Absolute(item.ItemSpec);
                if (!string.Equals(Path.GetExtension(original), ".json", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException($"Lottie source '{original}' must be a JSON file; format conversion is not supported.");
                var selected = searchFolders.Select(folder => Path.Combine(folder, Path.GetFileName(original)))
                    .Concat(new[] { original }).FirstOrDefault(File.Exists);
                if (selected == null)
                    throw new FileNotFoundException("Lottie source was not found.", original);
                selected = Path.GetFullPath(selected);
                var logical = item.GetMetadata("LogicalName");
                if (string.IsNullOrWhiteSpace(logical)) logical = item.GetMetadata("Link");
                if (string.IsNullOrWhiteSpace(logical)) logical = Path.GetFileName(original);
                logical = SafeRelative(logical);
                var bytes = File.ReadAllBytes(selected);
                byte[] prepared;
                try { prepared = optimize ? LottieJson.Minify(bytes) : bytes; }
                catch (JsonException ex) { throw new InvalidDataException($"Invalid Lottie JSON '{selected}': {ex.Message}", ex); }
                Add(outputs, selected, logical, prepared, item);

                var dependencies = item.GetMetadata("CompanionAssets").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                try
                {
                    using var document = JsonDocument.Parse(LottieJson.WithoutBom(bytes));
                    Discover(document.RootElement, dependencies, selected);
                }
                catch (JsonException) when (!optimize)
                {
                    build.Logger?.LogWarning($"Lottie '{selected}' is copied unchanged and cannot be inspected as JSON. Declare local dependencies in CompanionAssets.");
                }
                foreach (var dependency in dependencies.Distinct(StringComparer.Ordinal))
                {
                    var relative = SafeRelative(dependency);
                    var source = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(selected), Normalize(relative)));
                    if (!File.Exists(source))
                        throw new FileNotFoundException($"Lottie companion '{dependency}' referenced by '{selected}' was not found. Companions resolve beside the selected brand, not other search folders.", source);
                    // Do not allow a directory symlink to escape the selected source's directory.
                    RejectLinks(source, Path.GetDirectoryName(selected));
                    var name = SafeRelative((Path.GetDirectoryName(logical)?.Replace('\\', '/') ?? "") + "/" + relative, true);
                    Add(outputs, source, name, File.ReadAllBytes(source), new TaskItem(source));
                }
            }

            // Validate all declarations and collisions before replacing any previous good output.
            var metadata = declarations.Select(item => new { item.ItemSpec, Values = item.CloneCustomMetadata().Keys.Cast<string>()
                .OrderBy(x => x, StringComparer.Ordinal).ToDictionary(x => x, item.GetMetadata) }).ToArray();
            var fingerprint = ImageFingerprint.Create(new { OptimizeLottie = optimize, SearchFolders = searchFolders, Metadata = metadata, build.Configuration,
                Outputs = outputs.Values.Select(x => new { x.Source, x.LogicalName }).ToArray() },
                outputs.Values.Select(x => x.Source).Concat(new[] { configurationFile }));
            var previous = File.Exists(manifest) ? JsonSerializer.Deserialize<string[]>(File.ReadAllText(manifest)) : Array.Empty<string>();
            var results = new List<ITaskItem>();
            foreach (var output in outputs.Values)
            {
                var path = Path.Combine(root, "assets", Normalize(output.LogicalName));
                WriteChanged(path, output.Bytes);
                var result = new TaskItem(output.Item) { ItemSpec = path };
                result.SetMetadata("LogicalName", output.LogicalName);
                result.SetMetadata("Link", output.LogicalName);
                result.SetMetadata("TargetPath", output.LogicalName);
                result.SetMetadata("MobileBuildToolsPrepared", "true");
                result.SetMetadata("MobileBuildToolsOriginalSource", output.Source);
                results.Add(result);
                fileWrites.Add(path);
            }
            foreach (var stale in previous ?? Array.Empty<string>())
            {
                var safe = SafeRelative(stale);
                if (!outputs.ContainsKey(safe))
                {
                    var path = Path.Combine(root, "assets", Normalize(safe));
                    if (File.Exists(path)) File.Delete(path);
                }
            }
            WriteChanged(manifest, System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(outputs.Keys.OrderBy(x => x, StringComparer.Ordinal).ToArray())));
            WriteChanged(stamp, System.Text.Encoding.UTF8.GetBytes(fingerprint));
            fileWrites.Add(manifest);
            fileWrites.Add(stamp);
            return results.ToArray();
        }

        private static void Add(Dictionary<string, Output> outputs, string source, string name, byte[] bytes, ITaskItem item)
        {
            if (outputs.TryGetValue(name, out var existing))
            {
                if (existing.Source != source || !existing.Bytes.SequenceEqual(bytes))
                    throw new InvalidDataException($"Conflicting Lottie assets '{existing.Source}' and '{source}' package as '{name}'. Use distinct LogicalName directories.");
                return;
            }
            if (outputs.Keys.Any(key => key.StartsWith(name + "/", StringComparison.OrdinalIgnoreCase) || name.StartsWith(key + "/", StringComparison.OrdinalIgnoreCase)))
                throw new InvalidDataException($"Lottie output path conflicts with another asset: '{name}'.");
            outputs.Add(name, new Output { Source = source, LogicalName = name, Bytes = bytes, Item = item });
        }

        private void Discover(JsonElement element, List<string> dependencies, string source)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                // Iterate properties instead of collapsing duplicate names through a model.
                foreach (var property in element.EnumerateObject())
                {
                    if (property.Name == "assets" && property.Value.ValueKind == JsonValueKind.Array)
                        foreach (var asset in property.Value.EnumerateArray())
                        {
                            if (asset.ValueKind != JsonValueKind.Object) continue;
                            var folders = Strings(asset, "u").DefaultIfEmpty("");
                            foreach (var path in Strings(asset, "p"))
                                foreach (var folder in folders)
                                    Reference(path.StartsWith("data:", StringComparison.OrdinalIgnoreCase) ? path : folder + path, dependencies, source);
                        }
                    if (property.Name == "fPath" && property.Value.ValueKind == JsonValueKind.String)
                        Reference(property.Value.GetString(), dependencies, source);
                    Discover(property.Value, dependencies, source);
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
                foreach (var child in element.EnumerateArray()) Discover(child, dependencies, source);
        }

        private static IEnumerable<string> Strings(JsonElement element, string name) => element.EnumerateObject()
            .Where(x => x.Name == name && x.Value.ValueKind == JsonValueKind.String).Select(x => x.Value.GetString());

        private void Reference(string path, List<string> dependencies, string source)
        {
            if (string.IsNullOrEmpty(path) || path.StartsWith("data:", StringComparison.OrdinalIgnoreCase)) return;
            if (path.StartsWith("//", StringComparison.Ordinal) || Uri.TryCreate(path, UriKind.Absolute, out var uri) && !uri.IsFile)
            {
                build.Logger?.LogWarning($"Lottie '{source}' references remote resource '{path}'. It is retained unchanged and will not be downloaded or packaged.");
                return;
            }
            dependencies.Add(path);
        }

        private static void RejectLinks(string source, string root)
        {
            for (var path = source; path != root && path != null; path = Path.GetDirectoryName(path))
                if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
                    throw new InvalidDataException($"Lottie companion path cannot traverse a symbolic link: '{path}'.");
        }

        private static string SafeRelative(string path, bool removeLeadingSlash = false)
        {
            path = path.Replace('\\', '/');
            if (removeLeadingSlash) path = path.TrimStart('/');
            if (string.IsNullOrWhiteSpace(path) || path.StartsWith("/", StringComparison.Ordinal) || path.Contains(":"))
                throw new InvalidDataException($"Lottie asset path must be relative: '{path}'.");
            var parts = path.Split('/');
            if (parts.Any(x => string.IsNullOrWhiteSpace(x) || x == "." || x == ".." || x.IndexOfAny(new[] { '*', '?', '\0' }) >= 0))
                throw new InvalidDataException($"Lottie asset path contains an unsafe segment: '{path}'.");
            return path;
        }

        private static void WriteChanged(string path, byte[] bytes)
        {
            if (File.Exists(path) && File.ReadAllBytes(path).SequenceEqual(bytes)) return;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, bytes);
        }
        private string Absolute(string path) => Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(build.ProjectDirectory, Normalize(path)));
        private static string Normalize(string path) => path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
    }
}
