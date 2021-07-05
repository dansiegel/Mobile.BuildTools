using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Models.Settings;
using Mobile.BuildTools.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace Mobile.BuildTools.Tasks
{
    public class LocateBuildToolsConfigTask : Task
    {
        public string IsCrossTargetingBuild { get; set; }

        [Required]
        public string ProjectName { get; set; }

        [Required]
        public string ProjectDir { get; set; }

        public string SolutionDir { get; set; }

        [Required]
        public string TargetFrameworkIdentifier { get; set; }

        public string SdkShortFrameworkIdentifier { get; set; }

        [Output]
        public string BuildToolsConfigFilePath { get; private set; }

        [Output]
        public string LocatedSolutionDirectory { get; private set; }

        [Output]
        public bool EnableArtifactCopy { get; private set; }

        [Output]
        public bool EnableAutomaticVersioning { get; private set; }

        [Output]
        public bool EnableScssToCss { get; private set; }

        [Output]
        public bool EnableImageProcessing { get; private set; }

        [Output]
        public bool EnableReleaseNotes { get; private set; }

        [Output]
        public bool EnableSecrets { get; private set; }

        [Output]
        public bool EnableTemplateManifests { get; private set; }

        public override bool Execute()
        {
//#if DEBUG
//            if (!System.Diagnostics.Debugger.IsAttached)
//                System.Diagnostics.Debugger.Launch();
//#endif
            LocateSolution();
            BuildToolsConfigFilePath = ConfigHelper.GetConfigurationPath(ProjectDir);
            MigrateSecretsToSettings();
            ValidateConfigSchema();

            var crossTargetingProject = IsCrossTargeting();
            var platform = TargetFrameworkIdentifier.GetTargetPlatform();
            var isPlatformHead = platform != Platform.Unsupported;
            var configuration = ConfigHelper.GetConfig(BuildToolsConfigFilePath);

            // Only run these tasks for Android and iOS projects if they're not explicitly disabled
            EnableArtifactCopy = !crossTargetingProject && IsEnabled(configuration?.ArtifactCopy) && isPlatformHead;
            EnableAutomaticVersioning = !crossTargetingProject && IsEnabled(configuration?.AutomaticVersioning) && isPlatformHead;
            EnableImageProcessing = !crossTargetingProject && IsEnabled(configuration?.Images) && (platform == Platform.iOS || platform == Platform.Android);
            EnableTemplateManifests = !crossTargetingProject && IsEnabled(configuration?.Manifests) && isPlatformHead;
            EnableReleaseNotes = IsEnabled(configuration?.ReleaseNotes) && isPlatformHead && EnvironmentAnalyzer.IsInGitRepo(ProjectDir);

            // Only run this if it's not disabled and there are SCSS files
            EnableScssToCss = IsEnabled(configuration?.Css) && Directory.EnumerateFiles(ProjectDir, "*.scss", SearchOption.AllDirectories).Any();

            // Only run this if it is there is a configuration that enables it... or there is are secrets with no config
            EnableSecrets = ShouldEnableSecrets(configuration);

            return true;
        }

        internal void ValidateConfigSchema()
        {
            try
            {
                var path = Path.Combine(BuildToolsConfigFilePath, Constants.BuildToolsConfigFileName);
                Log.LogMessage($"Validating buildtools.json at the path: {path}");
                var generator = new JSchemaGenerator();
                var schema = generator.Generate(typeof(BuildToolsConfig));
                var config = JObject.Parse(File.ReadAllText(path));
                if (!config.IsValid(schema, out IList<string> errorMessages))
                {
                    foreach(var error in errorMessages)
                    {
                        Log.LogError("Invalid buildtools.json schema detected...");
                        Log.LogError(error);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("There was an unhandled exception while attempting to validate the buildtools.json.");
                Log.LogError(ex.Message);
            }
        }

#pragma warning disable CS0612 // Project Secrets is obsolete. This converts to the new AppSettings.
        internal void MigrateSecretsToSettings()
        {
            var config =  ConfigHelper.GetConfig(BuildToolsConfigFilePath, skipActivation: true);
            if (config.AppSettings is null)
                config.AppSettings = new Dictionary<string, IEnumerable<SettingsConfig>>();

            if (config.ProjectSecrets is not null && config.ProjectSecrets.Any())
            {
                config.ProjectSecrets.ForEach(x =>
                {
                    var projectName = x.Key;
                    var secretsConfig = x.Value;
                    var settings = new SettingsConfig
                    {
                        Accessibility = secretsConfig.Accessibility,
                        ClassName = secretsConfig.ClassName ?? "Secrets",
                        Delimiter = secretsConfig.Delimiter,
                        Namespace = secretsConfig.Namespace,
                        Prefix = secretsConfig.Prefix,
                        Properties = secretsConfig.Properties,
                        RootNamespace = secretsConfig.RootNamespace
                    };

                    if(config.AppSettings.ContainsKey(projectName))
                    {
                        if(!config.AppSettings[projectName].Any(x => x.ClassName == settings.ClassName))
                        {
                            var allSettings = new List<SettingsConfig>(config.AppSettings[projectName]);
                            allSettings.Add(settings);
                            config.AppSettings[projectName] = allSettings;
                        }
                    }
                    else
                    {
                        config.AppSettings[projectName] = new[] { settings };
                    }
                });
                config.ProjectSecrets = null;
                ConfigHelper.SaveConfig(config, BuildToolsConfigFilePath);
            }
        }
#pragma warning restore CS0612 // Project Secrets is obsolete. This converts to the new AppSettings.

        public static bool IsEnabled(ToolItem item)
        {
            if (item is null) return true;

            return !(item.Disable ?? false);
        }

        private bool IsCrossTargeting()
        {
            if (string.IsNullOrEmpty(IsCrossTargetingBuild))
                return false;

            if (bool.TryParse(IsCrossTargetingBuild, out var result))
                return result;

            return false;
        }

        private bool ShouldEnableSecrets(BuildToolsConfig config)
        {
            return config.AppSettings is not null && config.AppSettings.ContainsKey(ProjectName) && config.AppSettings[ProjectName].Any();
        }

        private void LocateSolution()
        {
            if (!string.IsNullOrEmpty(SolutionDir))
            {
                LocatedSolutionDirectory = SolutionDir;
                return;
            }

            LocatedSolutionDirectory = Utils.EnvironmentAnalyzer.LocateSolution(ProjectDir);
        }
    }
}
