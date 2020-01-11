using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Models.Secrets;
using Mobile.BuildTools.Tasks.Utils;
using Mobile.BuildTools.Utils;
using Newtonsoft.Json;

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
//            if (!Debugger.IsAttached)
//                Debugger.Launch();
//#endif
            LocateSolution();
            GetConfiguration();

            var crossTargetingProject = IsCrossTargeting();
            var platform = TargetFrameworkIdentifier.GetTargetPlatform();
            var isPlatformHead = platform != Platform.Unsupported;
            var configuration = ConfigHelper.GetConfig(BuildToolsConfigFilePath);

            // Only run these tasks for Android and iOS projects if they're not explicitly disabled
            EnableArtifactCopy = !crossTargetingProject && IsEnabled(configuration?.ArtifactCopy) && isPlatformHead;
            EnableAutomaticVersioning = !crossTargetingProject && IsEnabled(configuration?.AutomaticVersioning) && isPlatformHead;
            EnableImageProcessing = !crossTargetingProject && IsEnabled(configuration?.Images) && (platform == Platform.iOS || platform == Platform.Android);
            EnableTemplateManifests = !crossTargetingProject && IsEnabled(configuration?.Manifests) && isPlatformHead;
            EnableReleaseNotes = IsEnabled(configuration?.ReleaseNotes) && isPlatformHead;

            // Only run this if it's not disabled and there are SCSS files
            EnableScssToCss = IsEnabled(configuration?.Css) && Directory.EnumerateFiles(ProjectDir, "*.scss", SearchOption.AllDirectories).Any();

            // Only run this if it is there is a configuration that enables it... or there is are secrets with no config
            EnableSecrets = ShouldEnableSecrets(configuration);

            return true;
        }

        public static bool IsEnabled(ToolItem item)
        {
            if (item is null) return true;

            return !item.Disable;
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
            var secretsFileExists = File.Exists(Path.Combine(ProjectDir, Constants.SecretsJsonFileName));
            var secretsConfig = ConfigHelper.GetSecretsConfig(ProjectName, ProjectDir, config);

            if(secretsConfig is null)
            {
                // Intentionally disable this condition in CI incase somebody checked in the secrets.json
                return secretsFileExists && !CIBuildEnvironmentUtils.IsBuildHost;
            }

            return !secretsConfig.Disable;
        }

        private void LocateSolution()
        {
            if (!string.IsNullOrEmpty(SolutionDir))
            {
                LocatedSolutionDirectory = SolutionDir;
                return;
            }

            LocatedSolutionDirectory = LocateSolution(ProjectDir);
        }

        private void GetConfiguration()
        {
            var configPath = SolutionDir;
            if(!string.IsNullOrEmpty(configPath) && !ConfigHelper.Exists(configPath))
            {
                configPath = GetConfiguration(ProjectDir);
            }

            BuildToolsConfigFilePath = configPath;
        }

        private string GetConfiguration(string searchDirectory)
        {
            if (string.IsNullOrEmpty(searchDirectory)) return null;

            var configPath = Path.Combine(searchDirectory, "buildtools.json");
            if (File.Exists(configPath))
            {
                return searchDirectory;
            }
            else if (Path.IsPathRooted(searchDirectory))
            {
                GenerateDefaultConfig(SolutionDir);
                return SolutionDir;
            }

            var parentDirectory = Directory.GetParent(searchDirectory);
            return GetConfiguration(parentDirectory.FullName);
        }

        private string LocateSolution(string searchDirectory)
        {
            var solutionFiles = Directory.GetFiles(searchDirectory, "*.sln");
            if(solutionFiles.Length > 0)
            {
                return searchDirectory;
            }
            else if(Path.IsPathRooted(searchDirectory))
            {
                return searchDirectory;
            }

            return LocateSolution(Directory.GetParent(searchDirectory).FullName);
        }

        private void GenerateDefaultConfig(string path)
        {
            Log.LogMessage($"Auto-Generating BuildTools configuration at '{path}'");
            ConfigHelper.SaveDefaultConfig(path);
        }
    }
}
