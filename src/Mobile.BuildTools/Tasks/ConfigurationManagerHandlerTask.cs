using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Models.Configuration;
using Mobile.BuildTools.Tasks.Generators.AppConfig;

namespace Mobile.BuildTools.Tasks
{
    public class ConfigurationManagerHandlerTask : BuildToolsTaskBase
    {
        public ITaskItem[] InputConfigFiles { get; set; }

        public ITaskItem[] GeneratedAppConfig { get; set; }

        [Required]
        public string AppConfigEnvironment { get; set; }

        [Output]
        public ITaskItem[] OutputConfigs =>
            GeneratedAppConfig?.Select(x => new TaskItem(x.ItemSpec))?.ToArray()
                ?? Array.Empty<ITaskItem>();

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            // Validate we have inputs...
            if (InputConfigFiles is null || !InputConfigFiles.Any())
            {
                Log.LogMessage("No input config files were found");
                return;
            }

            if(GeneratedAppConfig is null || !GeneratedAppConfig.Any())
            {
                Log.LogError("Input app.config's were found, however no outputs were found.");
                return;
            }

            // Get root app.config
            var rootConfigFile = GetTaskItem(InputConfigFiles, "app.config");
            if (rootConfigFile is null)
            {
                // this should never happen...
                Log.LogError("We could not locate an 'app.config'. Please file a bug.");
                return;
            }

            // Reset output directory
            var fi = new FileInfo(GeneratedAppConfig.First().ItemSpec);
            if(fi.Directory.Exists)
            {
                fi.Directory.Delete(true);
            }
            fi.Directory.Create();

            // Get Transform config
            var transformFile = GetTaskItem(InputConfigFiles, $"app.{AppConfigEnvironment}.config");
            var outputs = GeneratedAppConfig.Select(x => x.ToExpectedAppConfig());
            var generator = new ConfigurationManagerTransformationGenerator(config)
            {
                BaseConfigPath = rootConfigFile.ItemSpec,
                TransformFilePath = transformFile.ItemSpec,
                ExpectedConfigs = outputs
            };
            generator.Execute();
        }

        private static ITaskItem GetTaskItem(IEnumerable<ITaskItem> items, string expectedFileName) =>
           items.FirstOrDefault(x =>
            Path.GetFileName(x.ItemSpec).Equals(expectedFileName, StringComparison.InvariantCultureIgnoreCase) ||
            (!string.IsNullOrEmpty(x.GetMetadata("Link")) && Path.GetFileName(x.GetMetadata("Link")).Equals(expectedFileName, StringComparison.InvariantCultureIgnoreCase)));
    }
}
