using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;

namespace Mobile.BuildTools.Tasks
{
    public class AppConfigCopyTask : BuildToolsTaskBase
    {
        private const string EmptyAppConfig = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  <appSettings>
  </appSettings>
  <connectionStrings>
  </connectionStrings>
</configuration>";

        public ITaskItem[] InputConfigFiles { get; set; }

        private List<ITaskItem> _outputs = new List<ITaskItem>();
        [Output]
        public ITaskItem[] OutputConfigs => _outputs.ToArray();

        private List<ITaskItem> _copiedConfigs = new List<ITaskItem>();
        [Output]
        public ITaskItem[] CopiedConfigs => _copiedConfigs.ToArray();

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            // if (!System.Diagnostics.Debugger.IsAttached)
            //     System.Diagnostics.Debugger.Launch();

            if (!InputConfigFiles.Any())
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

            var files = InputConfigFiles.Select(x => x.ItemSpec);
            var configsOutputDir = Path.Combine(IntermediateOutputPath, "configs");

            if (!Directory.Exists(configsOutputDir))
            {
                Directory.CreateDirectory(configsOutputDir);
            }

            foreach (var file in files)
            {
                CopyFile(configsOutputDir, file, config);
            }

            if(!_outputs.Any(x => Path.GetFileName(x.ItemSpec).Equals("app.config", StringComparison.InvariantCultureIgnoreCase)))
            {
                var projectFile = Path.Combine(config.ProjectDirectory, "app.config");
                File.WriteAllText(projectFile, EmptyAppConfig);
                CopyFile(configsOutputDir, projectFile, config);
            }
        }

        private void CopyFile(string configsOutputDir, string file, IBuildConfiguration config)
        {
            var outputFilePath = Path.Combine(configsOutputDir, Path.GetFileName(file));
            var lockFilePath = $"{outputFilePath}.lock";
            var inputFileInfo = new FileInfo(file);

            if (!File.Exists(outputFilePath) || !File.Exists(lockFilePath) ||
                DateTime.Parse(File.ReadAllText(lockFilePath)) < inputFileInfo.LastWriteTimeUtc)
            {
                config.Logger.LogMessage($"Copying {file} to {outputFilePath}");
                inputFileInfo.CopyTo(outputFilePath);
                File.WriteAllText(lockFilePath, $"{inputFileInfo.LastWriteTimeUtc.ToString()}");
            }

            _copiedConfigs.Add(new TaskItem(outputFilePath));
            var includeAllConfigs = config.Configuration.AppConfig.IncludeAllConfigs;
            if (includeAllConfigs || Path.GetFileName(file).Equals("app.config", StringComparison.InvariantCultureIgnoreCase))
            {
                _outputs.Add(new TaskItem(outputFilePath));
            }
        }
    }
}
