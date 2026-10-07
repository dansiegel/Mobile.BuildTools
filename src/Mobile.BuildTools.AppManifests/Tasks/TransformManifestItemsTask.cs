using System.Security.Cryptography;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Manifests;
using Mobile.BuildTools.Generators.Versioning;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks;

public class TransformManifestItemsTask : BuildToolsTaskBase
{
    public ITaskItem[] Manifests { get; set; } = [];
    [Required]
    public string OutputDirectory { get; set; }
    public bool ReplaceTokens { get; set; } = true;
    public bool ApplyPackageName { get; set; } = true;
    public bool VersionManifest { get; set; }
    [Output]
    public ITaskItem[] ProcessedManifests { get; private set; } = [];
    [Output]
    public ITaskItem[] GeneratedFiles { get; private set; } = [];

    [Output]
    public bool Changed { get; private set; }

    internal override void ExecuteInternal(IBuildConfiguration config)
    {
        var manifests = new List<ITaskItem>();
        var generated = new List<ITaskItem>();
        foreach (var item in Manifests)
        {
            var source = Path.GetFullPath(Path.Combine(ProjectDirectory, item.ItemSpec));
            if (!File.Exists(source))
            {
                Log.LogError($"Manifest file does not exist: '{item.ItemSpec}'.");
                continue;
            }
            var generator = ManifestGeneratorFactory.Create(source, config);
            if (generator == null)
            {
                Log.LogError($"Unsupported manifest format: '{Path.GetExtension(source)}'. Use JSON for PWA manifests, not AppManifest.js.");
                continue;
            }
            using var hash = SHA256.Create();
            var identity = BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(source))).Replace("-", "").Substring(0, 16);
            var output = Path.GetFullPath(Path.Combine(ProjectDirectory, OutputDirectory, identity, Path.GetFileName(source)));
            generator.ManifestInputPath = source;
            generator.ManifestOutputPath = output;
            generator.EnableTokenReplacement = ReplaceTokens;
            generator.ApplyPackageName = ApplyPackageName;
            var previous = File.Exists(output) ? File.ReadAllBytes(output) : null;
            generator.Execute();
            Changed |= previous == null || !previous.SequenceEqual(File.ReadAllBytes(output));
            generated.Add(new TaskItem(output));

            if (VersionManifest && config.Configuration.AutomaticVersioning.Behavior != VersionBehavior.Off &&
                !(CIBuildEnvironmentUtils.IsBuildHost && config.Configuration.AutomaticVersioning.Environment == VersionEnvironment.Local))
            {
                var versioned = Path.GetFullPath(Path.Combine(ProjectDirectory, OutputDirectory, identity, "versioned", Path.GetFileName(source)));
                BuildVersionGeneratorBase versioning = config.Platform switch
                {
                    Platform.Android => new AndroidAutomaticBuildVersionGenerator(config, output, versioned),
                    Platform.iOS or Platform.macOS or Platform.TVOS => new iOSAutomaticBuildVersionGenerator(config, output, versioned),
                    _ => null
                };
                if (versioning != null)
                {
                    versioning.Execute();
                    output = versioned;
                    generated.Add(new TaskItem(output));
                }
            }
            var processed = new TaskItem(item) { ItemSpec = output };
            processed.SetMetadata("MobileBuildToolsSourceManifest", source);
            manifests.Add(processed);
        }
        ProcessedManifests = manifests.ToArray();
        GeneratedFiles = generated.ToArray();
    }
}
