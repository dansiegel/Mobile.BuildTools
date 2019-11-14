using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Tasks.Generators.AppConfig;

namespace Mobile.BuildTools.Tasks.Tasks
{
    public class ConfigurationManagerHandlerTask : BuildToolsTaskBase
    {
        [Required]
        public string BuildConfiguration { get; set; }


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

                // TODO: We need to set the outputs.
                IGenerator generator = new ConfigurationManagerTransformationGenerator(config)
                {
                    BaseConfigPath = Path.Combine(IntermediateOutputPath, "app.config"),
                    BuildConfiguration = BuildConfiguration
                };
                generator.Execute();
            }
        }
    }
}
