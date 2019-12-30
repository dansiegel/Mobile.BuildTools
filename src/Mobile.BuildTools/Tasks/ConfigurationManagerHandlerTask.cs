using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Build;
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
                var configsOutputDir = Path.Combine(IntermediateOutputPath, "configs");
                if(!Directory.Exists(configsOutputDir))
                {
                    Directory.CreateDirectory(configsOutputDir);
                }

                Directory.EnumerateFiles(ProjectDirectory, "app*.config", SearchOption.TopDirectoryOnly)
                    .Select(file =>
                    {
                        var outputFile = Path.Combine(configsOutputDir, Path.GetFileName(file));
                        File.Copy(file, outputFile);
                        return file;
                    });

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
