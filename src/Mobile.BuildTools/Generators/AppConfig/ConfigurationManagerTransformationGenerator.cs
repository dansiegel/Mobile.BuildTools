using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Configuration;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Tasks.Generators.AppConfig
{
    public class ConfigurationManagerTransformationGenerator : IGenerator
    {
        // app.config
        public string BaseConfigPath { get; set; }

        public string BuildConfiguration { get; set; }

        public ILog Log { get; set; }
        public bool? DebugOutput { get; set; }

        public void Execute()
        {
            var parentDirectory = Directory.GetParent(BaseConfigPath);
            var configFileName = Path.GetFileNameWithoutExtension(BaseConfigPath);
            var transformationConfigPath = Path.Combine(parentDirectory.FullName, $"{configFileName}.{BuildConfiguration}{Path.GetExtension(BaseConfigPath)}");


            var updatedConfig = TransformationHelper.Transform(File.ReadAllText(BaseConfigPath),
                                                               File.ReadAllText(transformationConfigPath));
            if (updatedConfig is null) return;

            var outputFile = Path.Combine(BaseConfigPath, Path.GetFileName(BaseConfigPath));
            updatedConfig.Save(outputFile);

            if(DebugOutput ?? false)
            {
                Log.LogMessage("*************** Transformed app.config ******************************");
                Log.LogMessage(updatedConfig.ToString());
            }
        }
    }
}
