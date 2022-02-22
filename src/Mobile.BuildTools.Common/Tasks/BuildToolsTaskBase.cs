using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Models.Settings;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks
{
    public abstract class BuildToolsTaskBase : Task, IBuildConfiguration
    {
        string IBuildConfiguration.BuildConfiguration =>
            GetProperty("Configuration") ?? Configuration;

        bool IBuildConfiguration.BuildingInsideVisualStudio =>
            bool.TryParse(GetProperty("BuildingInsideVisualStudio"), out var inVS) && inVS;

        public string Configuration { get; set; }

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
        BuildToolsConfig IBuildConfiguration.Configuration => _config;

        Platform IBuildConfiguration.Platform =>
            TargetFrameworkIdentifier.GetTargetPlatform();

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "We are aborting the build if an Exception has been logged.")]
        public sealed override bool Execute()
        {
            try
            {
                LocateSolution();

                ConfigurationPath = ConfigHelper.GetConfigurationPath(ProjectDirectory);
                _config = ConfigHelper.GetConfig(ConfigurationPath);
//#if DEBUG
//                if (!Debugger.IsAttached && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
//                    Debugger.Launch();
//#endif
                ExecuteInternal(this);
            }
            catch (Exception ex)
            {
#if DEBUG
                if (!Debugger.IsAttached && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    Debugger.Launch();
#endif
                Log.LogError($"Unhandled error while executing {GetType().Name}");
                Log.LogErrorFromException(ex);

                if(ConfigHelper.Exists(ConfigurationPath)
                    && ConfigHelper.GetConfig(ConfigurationPath).Debug)
                {
                    Log.LogWarning("**************** DEBUG OUTPUT ****************");
                    Log.LogWarning(ex.ToString());
                }
            }

            return !Log.HasLoggedErrors;
        }

        void IBuildConfiguration.SaveConfiguration() =>
            ConfigHelper.SaveConfig(((IBuildConfiguration)this).Configuration, ConfigurationPath);

        internal abstract void ExecuteInternal(IBuildConfiguration config);

        IEnumerable<SettingsConfig> IBuildConfiguration.GetSettingsConfig() =>
            ConfigHelper.GetSettingsConfig(this);

        private string GetProperty(string name)
        {
            var buildConfig = (IBuildConfiguration)this;
            if(buildConfig.GlobalProperties.ContainsKey(name))
                return buildConfig.GlobalProperties[name];

            return null;
        }

        private void LocateSolution()
        {
            if (!string.IsNullOrEmpty(SolutionDirectory) && Directory.EnumerateFiles(SolutionDirectory, "*.sln").Any())
            {
                return;
            }

            SolutionDirectory = Utils.EnvironmentAnalyzer.LocateSolution(ProjectDirectory);
        }
    }
}
