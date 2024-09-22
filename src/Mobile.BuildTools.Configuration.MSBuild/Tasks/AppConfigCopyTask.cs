using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Configuration;

namespace Mobile.BuildTools.Tasks;

public class AppConfigCopyTask : BuildToolsTaskBase
{
    public ITaskItem[] InputConfigFiles { get; set; }

    private List<ITaskItem> _outputs = [];
    [Output]
    public ITaskItem[] OutputConfigs => [.. _outputs];

    internal override void ExecuteInternal(IBuildConfiguration config)
    {
        // Validate Inputs
        if (InputConfigFiles is null || !InputConfigFiles.Any())
        {
            Log.LogMessage("No input config files were found");
            return;
        }

        var badInputs = InputConfigFiles.Where(x => !x.ItemSpec.EndsWith(".config"));
        if (badInputs.Any())
        {
            foreach (var item in badInputs)
                Log.LogError("Found invalid Config File input. Config files must end with the extension '.config': {0}", item.ItemSpec);
            return;
        }

        // Ensure there is an app.config available
        if (!InputConfigFiles.Any(x => FileNameEquals("app.config", x.ItemSpec) ||
            (!string.IsNullOrEmpty(x.GetMetadata("Link")) && FileNameEquals("app.config", x.GetMetadata("Link")))))
        {
            Log.LogError("You must have at least one file included with the build action MobileBuildToolsConfig with the file name app.config");
            return;
        }

        var configsOutputDir = Path.Combine(IntermediateOutputPath, "Mobile.BuildTools", "configs");

        // Determine which files need outputs
        //var files = InputConfigFiles.Select(x => x.ItemSpec);
        foreach (var item in InputConfigFiles)
        {
            CollectValidOutput(configsOutputDir, item, config);
        }
    }

    private static bool FileNameEquals(string expected, string actualPath) =>
        Path.GetFileName(actualPath).Equals(expected, StringComparison.InvariantCultureIgnoreCase);

    private void CollectValidOutput(string configsOutputDir, ITaskItem item, IBuildConfiguration config)
    {
        var filePath = item.ItemSpec;
        var link = item.GetMetadata(MetaData.Link);
        var fileName = !string.IsNullOrEmpty(link) ? Path.GetFileName(link) : Path.GetFileName(filePath);
        var outputFilePath = Path.Combine(configsOutputDir, fileName);
        var outputItem = new TaskItem(outputFilePath);
        outputItem.SetMetadata(MetaData.Link, link);
        outputItem.SetMetadata(MetaData.SourceFile, filePath);

        switch (config.Configuration.AppConfig.Strategy)
        {
            case Models.AppConfigStrategy.BundleAll:
                _outputs.Add(outputItem);
                break;
            case Models.AppConfigStrategy.BundleNonStandard:
                var standardConfigs = new[]
                {
                    "app.debug.config",
                    "app.release.config",
                    "app.store.config",
                    "app.adhoc.config"
                };
                if(!standardConfigs.Any(x => x.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    _outputs.Add(outputItem);
                }
                break;
            default:
                if (fileName.Equals("app.config", StringComparison.InvariantCultureIgnoreCase))
                {
                    _outputs.Add(outputItem);
                }
                break;
        }
    }
}
