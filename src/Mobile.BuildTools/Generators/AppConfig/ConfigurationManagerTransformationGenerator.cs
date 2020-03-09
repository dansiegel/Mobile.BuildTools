using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Configuration;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Tasks.Generators.AppConfig
{
    internal class ConfigurationManagerTransformationGenerator : GeneratorBase<IEnumerable<ITaskItem>>
    {
        public ConfigurationManagerTransformationGenerator(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        // C:/repos/MyProject/MyProject.iOS/app.config
        public string BaseConfigPath { get; set; }

        // C:/repos/MyProject/MyProject.iOS/app.config
        public string TransformFilePath { get; set; }

        protected override void ExecuteInternal()
        {
            var parentDirectory = Directory.GetParent(BaseConfigPath);
            var configFileName = Path.GetFileNameWithoutExtension(BaseConfigPath);

            if (!string.IsNullOrEmpty(TransformFilePath) && File.Exists(TransformFilePath))
            {
                var updatedConfig = TransformationHelper.Transform(File.ReadAllText(BaseConfigPath),
                                                               File.ReadAllText(TransformFilePath));
                if (updatedConfig is null) return;

                updatedConfig.Save(BaseConfigPath);

                if (Build.Configuration.Debug)
                {
                    Log.LogMessage("*************** Transformed app.config ******************************");
                    Log.LogMessage(updatedConfig.ToString());
                }
            }

            switch(Build.Configuration.AppConfig.Strategy)
            {
                case Models.AppConfigStrategy.BundleAll:
                    Outputs = parentDirectory.EnumerateFiles()
                                             .Select(x => new TaskItem(x.FullName));
                    break;
                case Models.AppConfigStrategy.BundleNonStandard:
                    var standardConfigs = new[]
                    {
                        "app.debug.config",
                        "app.release.config",
                        "app.store.config",
                        "app.adhoc.config"
                    };
                    Outputs = parentDirectory.EnumerateFiles()
                                             .Where(x => !standardConfigs.Any(s => s.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase)))
                                             .Select(x => new TaskItem(x.FullName));
                    break;
                default:
                    Outputs = new[] { new TaskItem(BaseConfigPath) };
                    break;
            }
        }
    }
}
