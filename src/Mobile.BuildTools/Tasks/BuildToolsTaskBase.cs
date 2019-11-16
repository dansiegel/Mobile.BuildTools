using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Models.Secrets;
using Mobile.BuildTools.Tasks.Utils;
using Mobile.BuildTools.Utils;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Tasks
{
    public abstract class BuildToolsTaskBase : Task, IBuildConfiguration
    {
        string IBuildConfiguration.BuildConfiguration =>
            GetProperty("Configuration");

        bool IBuildConfiguration.BuildingInsideVisualStudio =>
            bool.TryParse(GetProperty("BuildingInsideVisualStudio"), out var inVS) ? inVS : false;

        [Required]
        public string ConfigurationPath { get; set; }

        [Required]
        public string ProjectDirectory { get; set; }

        [Required]
        public string ProjectName { get; set; }

        [Required]
        public string SolutionDirectory { get; set; }

        [Required]
        public string IntermediateOutputPath { get; set; }

        [Required]
        public string TargetFrameworkIdentifier { get; set; }

        IDictionary<string, string> IBuildConfiguration.GlobalProperties => BuildEngine.GetGlobalProperties();

        ILog IBuildConfiguration.Logger =>
            (BuildHostLoggingHelper)Log;

        private BuildToolsConfig _config;
        BuildToolsConfig IBuildConfiguration.Configuration =>
            _config ?? (_config = ConfigHelper.GetConfig(ConfigurationPath));

        Platform IBuildConfiguration.Platform =>
            TargetFrameworkIdentifier.GetTargetPlatform();

        public sealed override bool Execute()
        {
            try
            {
#if DEBUG
                if (!Debugger.IsAttached)
                    Debugger.Launch();
#endif
                ExecuteInternal(this);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                Log.LogError($"Unhandled error while executing {GetType().Name}");
                Log.LogErrorFromException(ex);
            }

            return !Log.HasLoggedErrors;
        }

        void IBuildConfiguration.SaveConfiguration() =>
            ConfigHelper.SaveConfig(((IBuildConfiguration)this).Configuration, ConfigurationPath);

        internal abstract void ExecuteInternal(IBuildConfiguration config);

        SecretsConfig IBuildConfiguration.GetSecretsConfig()
        {
            var configPath = Path.Combine(ProjectDirectory, "secrets.config");
            if (File.Exists(configPath))
            {
                return JsonConvert.DeserializeObject<SecretsConfig>(configPath);
            }

            var build = (IBuildConfiguration)this;
            var config = build.Configuration;
            if (config.ProjectSecrets != null && config.ProjectSecrets.Any(x => x.Key == ProjectName))
                return config.ProjectSecrets.First(x => x.Key == ProjectName).Value;

            return null;
        }

        private string GetProperty(string name)
        {
            var buildConfig = (IBuildConfiguration)this;
            if(buildConfig.GlobalProperties.ContainsKey(name))
                return buildConfig.GlobalProperties[name];

            return null;
        }
    }
}
