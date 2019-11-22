using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Tasks.Generators.AppConfig;

namespace Mobile.BuildTools.Tasks
{
    public class ConfigurationManagerHandlerTask : BuildToolsTaskBase
    {
        private List<ITaskItem> _outputs = new List<ITaskItem>();
        [Output]
        public ITaskItem[] OutputConfigs => _outputs.ToArray();

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            if (File.Exists(Path.Combine(ProjectDirectory, "app.config")))
            {
                foreach (var file in Directory.EnumerateFiles(ProjectDirectory, "app*.config", SearchOption.TopDirectoryOnly))
                {
                    var outputFile = Path.Combine(IntermediateOutputPath, "configs", Path.GetFileName(file));
                    File.Copy(file, outputFile);
                }

                var generator = new ConfigurationManagerTransformationGenerator(config)
                {
                    BaseConfigPath = Path.Combine(IntermediateOutputPath, "app.config")
                };
                generator.Execute();

                _outputs.AddRange(generator.Outputs);
            }
        }
    }
}
