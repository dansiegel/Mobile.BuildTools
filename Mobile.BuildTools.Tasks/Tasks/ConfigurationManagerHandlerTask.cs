using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Tasks.Generators;

namespace Mobile.BuildTools.Tasks.Tasks
{
    public class ConfigurationManagerHandlerTask : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string ProjectDirectory { get; set; }

        [Required]
        public string BuildConfiguration { get; set; }

        [Required]
        public string IntermediateOutputPath { get; set; }

        public string DebugOutput { get; set; }


        private List<ITaskItem> _outputs = new List<ITaskItem>();
        [Output]
        public ITaskItem[] OutputConfigs => _outputs.ToArray();

        public override bool Execute()
        {
            try
            {
                if(File.Exists(Path.Combine(ProjectDirectory, "app.config")))
                {
                    foreach (var file in Directory.EnumerateFiles(ProjectDirectory, "app*.config", SearchOption.TopDirectoryOnly))
                    {
                        var outputFile = Path.Combine(IntermediateOutputPath, "configs", Path.GetFileName(file));
                        File.Copy(file, outputFile);
                    }

                    var generator = new ConfigurationManagerTransformationGenerator
                    {
                        BaseConfigPath = Path.Combine(IntermediateOutputPath, "app.config"),
                        BuildConfiguration = BuildConfiguration,
                        DebugOutput = bool.TryParse(DebugOutput, out var debug) && debug,
                        Log = (BuildHostLoggingHelper)Log
                    };
                    generator.Execute();
                }
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
            }

            return !Log.HasLoggedErrors;
        }
    }
}
