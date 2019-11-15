using System.Diagnostics;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks
{
    public class LocateBuildToolsConfigTask : Task
    {
        public string ProjectDir { get; set; }

        public string SolutionDir { get; set; }

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
#if DEBUG
            if (!Debugger.IsAttached)
                Debugger.Launch();
#endif
            LocateSolution();
            GetConfiguration();

            var configuration = ConfigHelper.GetConfig(BuildToolsConfigFilePath);
            EnableArtifactCopy = !configuration.ArtifactCopy.Disable;
            EnableAutomaticVersioning = !configuration.AutomaticVersioning.Disable;
            EnableScssToCss = !configuration.Css.Disable;
            EnableImageProcessing = !configuration.Images.Disable;
            EnableSecrets = true; // TODO: We should look up some sort of config for this
            EnableTemplateManifests = !configuration.Manifests.Disable;
            EnableReleaseNotes = !configuration.ReleaseNotes.Disable;

            return true;
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
