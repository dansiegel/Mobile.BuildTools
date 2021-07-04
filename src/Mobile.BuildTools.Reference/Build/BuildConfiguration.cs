using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Models.Settings;
using Mobile.BuildTools.Reference.Models.Settings;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Build
{
    internal class BuildConfiguration : IBuildConfiguration
    {
        private string _configPath { get; }

        public BuildConfiguration(string projectDirectory, string intermediateOutputDirectory, Platform platform, ILog logger)
        {
            ProjectDirectory = projectDirectory;
            IntermediateOutputPath = intermediateOutputDirectory;
            Platform = platform;
            Logger = logger ?? new ConsoleLogger();
            SolutionDirectory = LocateSolution(null);

            _configPath = ConfigHelper.GetConfigurationPath(ProjectDirectory);
            Configuration = ConfigHelper.GetConfig(_configPath);
        }

        public string ProjectDirectory { get; }
        public string IntermediateOutputPath { get; }
        public Platform Platform { get; }
        public ILog Logger { get; }

        public bool BuildingInsideVisualStudio { get; }
        public IDictionary<string, string> GlobalProperties { get; }
        public string ProjectName { get; }
        public string SolutionDirectory { get; }
        public BuildToolsConfig Configuration { get; }
        string IBuildConfiguration.BuildConfiguration { get; }

        public IEnumerable<SettingsConfig> GetSettingsConfig() =>
            ConfigHelper.GetSettingsConfig(this);

        public void SaveConfiguration() => 
            ConfigHelper.SaveConfig(Configuration, _configPath);

        private string LocateSolution(string solutionDirectory)
        {
            if (!string.IsNullOrEmpty(solutionDirectory) && Directory.EnumerateFiles(solutionDirectory, "*.sln").Any())
            {
                return null;
            }

            return EnvironmentAnalyzer.LocateSolution(ProjectDirectory);
        }
    }
}
