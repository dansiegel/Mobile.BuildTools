using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Drawing;
using Mobile.BuildTools.Models.AppIcons;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Generators.Images
{
    /// <summary>Prepares sources for MAUI/Uno. Framework resizetizers still own all platform outputs.</summary>
    internal sealed class NativeImagePreparer
    {
        private readonly IBuildConfiguration build;
        private readonly string root;
        private readonly string[] searchFolders;
        private readonly string configurationFile;
        private readonly List<string> fileWrites = new List<string>();

        public NativeImagePreparer(IBuildConfiguration build, string targetFramework, string runtimeIdentifier, IEnumerable<string> searchFolders, string configurationFile)
        {
            this.build = build;
            this.configurationFile = File.Exists(configurationFile) ? configurationFile : null;
            this.searchFolders = searchFolders.Where(Directory.Exists).Select(Path.GetFullPath).Distinct().ToArray();
            root = Path.Combine(Absolute(build.IntermediateOutputPath), "Mobile.BuildTools", "native-images",
                Segment(build.BuildConfiguration), Segment(targetFramework), Segment(runtimeIdentifier),
                ImageFingerprint.Hash(Path.GetFullPath(build.ProjectDirectory)).Substring(0, 16));
        }

        public IReadOnlyList<string> FileWrites => fileWrites;

        public ITaskItem Prepare(ITaskItem item)
        {
            // Items exported from referenced projects have already been prepared in their own context.
            if (item.GetMetadata("MobileBuildToolsPrepared") == "true")
                return new TaskItem(item);

            var source = Absolute(item.ItemSpec);
            var kind = item.GetMetadata("MobileBuildToolsItemType");
            var selectedSource = FindSource(source);
            var definition = ReadDefinition(selectedSource, source, out var definitionFile);
            var resources = definition.GetConfigurations(build.Platform).ToArray();
            var resource = resources[0];
            if (resource.Ignore)
                return null;
            if (resources.Length > 1)
                build.Logger.LogWarning($"Additional outputs for '{source}' are ignored by native image preparation; declare additional framework items instead.");

            var result = new TaskItem(item);
            var customMetadata = item.CloneCustomMetadata();
            var metadata = customMetadata.Keys.Cast<string>().OrderBy(x => x, StringComparer.Ordinal)
                .ToDictionary(x => x, x => (string)customMetadata[x]);
            var dependencies = new List<string> { selectedSource, definitionFile, configurationFile };
            var foreground = item.GetMetadata("ForegroundFile");
            if (!string.IsNullOrEmpty(foreground))
            {
                var foregroundSource = FindSource(Absolute(foreground));
                dependencies.Add(foregroundSource);
                // Foregrounds retain their own sidecar settings and the item's foreground scale/color metadata.
                var foregroundItem = new TaskItem(Absolute(foreground));
                foregroundItem.SetMetadata("MobileBuildToolsItemType", kind + "-foreground");
                var preparedForeground = Prepare(foregroundItem);
                if (preparedForeground is null)
                    throw new InvalidDataException($"The foreground image '{foreground}' cannot be ignored while its parent is included.");
                result.SetMetadata("ForegroundFile", preparedForeground.ItemSpec);
                dependencies.Add(preparedForeground.ItemSpec);
                dependencies.Add(preparedForeground.ItemSpec + ".inputs");
            }

            var watermark = ResolveWatermark(resource.Watermark, definitionFile, selectedSource);
            dependencies.Add(watermark?.SourceFile);
            dependencies.Add(watermark?.FontFile);
            var needsRendering = resource.Width.GetValueOrDefault() > 0 || resource.Height.GetValueOrDefault() > 0 ||
                (resource.Scale > 0 && resource.Scale != 1) || (resource.PaddingFactor.HasValue && resource.PaddingFactor != 0 && resource.PaddingFactor != 1) ||
                !string.IsNullOrWhiteSpace(resource.BackgroundColor) || watermark != null;
            var extension = needsRendering ? ".png" : Path.GetExtension(selectedSource);
            // Keep the logical source filename, even when a branded replacement uses another format.
            var output = Path.Combine(root, Segment(kind), ImageFingerprint.Hash(source).Substring(0, 16),
                Path.GetFileNameWithoutExtension(source) + extension);
            var fingerprint = ImageFingerprint.Create(new { Resource = resource, Watermark = watermark, Metadata = metadata, build.Configuration }, dependencies);
            if (!ImageFingerprint.IsCurrent(output, fingerprint))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(output));
                if (needsRendering)
                {
                    new ImageResizeGenerator(build).ProcessImage(new OutputImage
                    {
                        InputFile = selectedSource,
                        OutputFile = output,
                        Width = resource.Width.GetValueOrDefault(),
                        Height = resource.Height.GetValueOrDefault(),
                        Scale = NormalizeScale(resource.Scale),
                        BackgroundColor = resource.BackgroundColor,
                        PaddingColor = resource.PaddingColor,
                        PaddingFactor = resource.PaddingFactor,
                        Watermark = watermark
                    });
                }
                else
                {
                    File.Copy(selectedSource, output, true);
                    // A changed config/foreground must invalidate timestamp-based framework targets too.
                    File.SetLastWriteTimeUtc(output, DateTime.UtcNow);
                }
                File.WriteAllText(output + ".inputs", fingerprint);
            }

            fileWrites.Add(output);
            fileWrites.Add(output + ".inputs");
            result.ItemSpec = output;
            // Frameworks resize SVGs by default, but not PNGs. Preserve that behavior after
            // rendering a vector (or selecting a bitmap brand for a vector declaration).
            var vectorSource = string.Equals(Path.GetExtension(source), ".svg", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(Path.GetExtension(selectedSource), ".svg", StringComparison.OrdinalIgnoreCase);
            if (vectorSource && !string.Equals(extension, ".svg", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(result.GetMetadata("Resize")))
                    result.SetMetadata("Resize", "true");
                if (string.IsNullOrEmpty(result.GetMetadata("BaseSize")) &&
                    !string.Equals(result.GetMetadata("Resize"), "false", StringComparison.OrdinalIgnoreCase))
                {
                    using var prepared = ImageBase.Load(output);
                    var size = prepared.GetOriginalSize();
                    result.SetMetadata("BaseSize", $"{size.Width},{size.Height}");
                }
            }
            // Link identifies the resource in native pipelines. Keep explicit metadata exactly as supplied.
            if (kind.EndsWith("Image", StringComparison.Ordinal) && string.IsNullOrEmpty(item.GetMetadata("Link")))
                result.SetMetadata("Link", Path.ChangeExtension(Path.IsPathRooted(item.ItemSpec) ? Path.GetFileName(item.ItemSpec) : item.ItemSpec, extension));
            result.SetMetadata("MobileBuildToolsPrepared", "true");
            result.SetMetadata("MobileBuildToolsOriginalSource", source);
            return result;
        }

        private ResourceDefinition ReadDefinition(string selectedSource, string originalSource, out string definitionFile)
        {
            var name = Path.GetFileNameWithoutExtension(originalSource) + ".json";
            definitionFile = searchFolders.Select(x => Path.Combine(x, name))
                .Concat(new[] { Path.ChangeExtension(selectedSource, ".json"), Path.ChangeExtension(originalSource, ".json") })
                .FirstOrDefault(File.Exists);
            var definition = definitionFile is null ? new ResourceDefinition() :
                JsonSerializer.Deserialize<ResourceDefinition>(File.ReadAllText(definitionFile), ConfigHelper.GetSerializerSettings()) ?? new ResourceDefinition();
            definition.SourceFile = selectedSource;
            return definition;
        }

        private WatermarkConfiguration ResolveWatermark(WatermarkConfiguration configuration, string definitionFile, string source)
        {
            if (configuration is null)
                return null;
            return new WatermarkConfiguration
            {
                SourceFile = ResolveDependency(configuration.SourceFile, definitionFile, source, true),
                FontFile = ResolveDependency(configuration.FontFile, definitionFile, source, false),
                FontFamily = configuration.FontFamily,
                Colors = configuration.Colors,
                Position = configuration.Position,
                Text = configuration.Text,
                TextColor = configuration.TextColor,
                Opacity = configuration.Opacity
            };
        }

        private string ResolveDependency(string path, string definitionFile, string source, bool allowImageStem)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;
            var roots = searchFolders.Concat(new[] { Path.GetDirectoryName(definitionFile ?? source), build.ProjectDirectory });
            foreach (var folder in roots)
            {
                var candidate = Path.IsPathRooted(path) ? path : Path.Combine(folder, Normalize(path));
                if (File.Exists(candidate))
                    return Path.GetFullPath(candidate);
                if (allowImageStem && !Path.HasExtension(path))
                {
                    var match = FindImageInFolder(Path.GetDirectoryName(candidate), Path.GetFileName(candidate));
                    if (match != null)
                        return match;
                }
            }
            throw new FileNotFoundException("Watermark dependency was not found.", path);
        }

        private string FindSource(string source)
        {
            foreach (var folder in searchFolders)
            {
                var exact = Path.Combine(folder, Path.GetFileName(source));
                if (File.Exists(exact))
                    return exact;
                var match = FindImageInFolder(folder, Path.GetFileNameWithoutExtension(source));
                if (match != null)
                    return match;
            }
            if (!File.Exists(source))
                throw new FileNotFoundException("Native image source was not found.", source);
            return source;
        }

        private static string FindImageInFolder(string folder, string stem)
        {
            if (!Directory.Exists(folder))
                return null;
            var extensions = new[] { ".png", ".svg", ".jpg", ".jpeg", ".gif", ".webp", ".bmp" };
            var matches = Directory.EnumerateFiles(folder).Where(x => Path.GetFileNameWithoutExtension(x).Equals(stem, StringComparison.OrdinalIgnoreCase) &&
                extensions.Contains(Path.GetExtension(x), StringComparer.OrdinalIgnoreCase)).OrderBy(x => x, StringComparer.Ordinal).ToArray();
            if (matches.Length > 1)
                throw new InvalidDataException($"Multiple branded sources match '{stem}' in '{folder}'. Use a unique source filename.");
            return matches.FirstOrDefault();
        }

        private static double NormalizeScale(double scale)
        {
            while (scale > 1 && !double.IsInfinity(scale))
                scale /= 100;
            return scale;
        }

        private string Absolute(string path) => Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(build.ProjectDirectory, Normalize(path)));
        private static string Normalize(string path) => path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        private static string Segment(string value) => string.IsNullOrEmpty(value) ? "default" :
            new string(value.Select(x => char.IsLetterOrDigit(x) || x == '.' || x == '-' || x == '_' ? x : '_').ToArray());
    }
}
