using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Tasks
{
    public class TemplateManifestTask : Microsoft.Build.Utilities.Task
    {
        public string Prefix { get; set; }

        public string Token { get; set; }

        public string ManifestTemplatePath { get; set; }

        public string ManifestOutputPath { get; set; }

        public string SdkShortFrameworkIdentifier { get; set; }

        public override bool Execute()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ManifestOutputPath) || string.IsNullOrWhiteSpace(ManifestTemplatePath))
                {
                    Log.LogMessage($"No Template path specified for {SdkShortFrameworkIdentifier}");
                }
                else if (File.Exists(ManifestOutputPath))
                {
                    Log.LogMessage($"'{Path.GetFileName(ManifestOutputPath)}' already exists.");
                }
                else if (File.Exists(ManifestTemplatePath))
                {
                    Log.LogMessage($"Generating '{Path.GetFileName(ManifestOutputPath)}'");
                    var generator = new AppManifestGenerator()
                    {
                        Prefix = Prefix,
                        Token = Token,
                        ManifestOutputPath = ManifestOutputPath,
                        ManifestTemplatePath = ManifestTemplatePath,
                        Log = (BuildHostLoggingHelper)Log,
                    };

                    generator.Execute();
                }
                else
                {
                    Log.LogWarning($"Could not locate template manifest at '{ManifestTemplatePath}'");
                }
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);

                return false;
            }
            return true;
        }
    }
}