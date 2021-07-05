using System.Collections.Generic;
using System.Linq;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Models.Settings;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks
{
    public class BuildEnvironmentDumpTask : Microsoft.Build.Utilities.Task
    {
        public string ProjectDirectory { get; set; }

        public string Configuration { get; set; }

        public override bool Execute()
        {
            var config = new BuildConfig(Configuration, ProjectDirectory);
            var variables = Utils.EnvironmentAnalyzer.GatherEnvironmentVariables(config, true);

            string message;
            if (variables.Any())
            {
                message = $"Found {variables.Count} Environment Variables\n";
                foreach(var variable in variables)
                {
                    message += $"    {variable.Key}: {variable.Value}\n";
                }
            }
            else
            {
                message = "There doesn't seem to be any Environment Variables... something is VERY VERY Wrong...";
            }

            Log.LogMessage(Microsoft.Build.Framework.MessageImportance.High, message);

            return true;
        }

        private class BuildConfig : IBuildConfiguration
        {
            public BuildConfig(string configuration, string projectDir)
            {
                BuildConfiguration = configuration;
                ProjectDirectory = projectDir;
                SolutionDirectory = EnvironmentAnalyzer.LocateSolution(projectDir);
                Configuration = ConfigHelper.GetConfig(projectDir);
            }

            public string BuildConfiguration { get; }
            public bool BuildingInsideVisualStudio { get; }
            public IDictionary<string, string> GlobalProperties { get; }
            public string ProjectName { get; }
            public string ProjectDirectory { get; }
            public string SolutionDirectory { get; }
            public string IntermediateOutputPath { get; }
            public ILog Logger { get; }
            public BuildToolsConfig Configuration { get; }
            public Platform Platform { get; }

            public IEnumerable<SettingsConfig> GetSettingsConfig()
            {
                throw new System.NotImplementedException();
            }

            public void SaveConfiguration()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
