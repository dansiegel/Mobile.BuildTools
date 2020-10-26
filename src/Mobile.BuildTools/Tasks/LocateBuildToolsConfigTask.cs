using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Models;
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
//            if (!System.Diagnostics.Debugger.IsAttached)
//                System.Diagnostics.Debugger.Launch();
//#endif
            LocateSolution();
            BuildToolsConfigFilePath = ConfigHelper.GetConfigurationPath(ProjectDir);

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

            LocatedSolutionDirectory = Utils.EnvironmentAnalyzer.LocateSolution(ProjectDir);
        }
    }
}
