using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Configuration;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Models.Configuration;

namespace Mobile.BuildTools.Tasks.Generators.AppConfig;

internal class ConfigurationManagerTransformationGenerator : GeneratorBase
{
    public ConfigurationManagerTransformationGenerator(IBuildConfiguration buildConfiguration)
        : base(buildConfiguration)
    {
    }

    // C:/repos/MyProject/MyProject.iOS/app.config
    public string BaseConfigPath { get; set; }

    // C:/repos/MyProject/MyProject.iOS/app.config
    public string TransformFilePath { get; set; }

    public IEnumerable<ExpectedAppConfig> ExpectedConfigs { get; set; }

    protected override void ExecuteInternal()
    {
        // Copy all output files...
        foreach (var config in ExpectedConfigs)
        {
            Log.LogMessage($"Copying '{config.SourceFile}' to '{config.OutputPath}'.");
            File.Copy(config.SourceFile, config.OutputPath);
        }

        if (!string.IsNullOrEmpty(TransformFilePath) && File.Exists(TransformFilePath))
        {
            var updatedConfig = TransformationHelper.Transform(File.ReadAllText(BaseConfigPath),
                                                               File.ReadAllText(TransformFilePath));
            if (updatedConfig is null) return;

            var appConfig = ExpectedConfigs.First(x => Path.GetFileName(x.OutputPath) == "app.config");
            updatedConfig.Save(appConfig.OutputPath);

            if (Build.Configuration.Debug)
            {
                Log.LogMessage("*************** Transformed app.config ******************************");
                Log.LogMessage(updatedConfig.ToString());
            }
        }
    }
}
