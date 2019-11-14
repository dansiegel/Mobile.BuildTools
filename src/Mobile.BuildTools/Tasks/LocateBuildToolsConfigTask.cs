using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks
{
    public class LocateBuildToolsConfigTask : Task
    {
        private ITaskItem _configuration;
        private ITaskItem _solutionDirectory;

        public string ProjectDir { get; set; }

        public string SolutionDir { get; set; }

        [Output]
        public ITaskItem Configuration => _configuration;

        [Output]
        public ITaskItem LocatedSolutionDirectory => _solutionDirectory;

        public override bool Execute()
        {
            LocateSolution();
            GetConfiguration();
            return true;
        }

        private void LocateSolution()
        {
            if (!string.IsNullOrEmpty(SolutionDir))
                return;

            SolutionDir = LocateSolution(ProjectDir);
            _solutionDirectory = new TaskItem(SolutionDir);
        }

        private void GetConfiguration()
        {
            var configPath = SolutionDir;
            if(!ConfigHelper.Exists(configPath))
            {
                configPath = GetConfiguration(ProjectDir);
            }

            _configuration = new TaskItem(configPath);
        }

        private string GetConfiguration(string searchDirectory)
        {
            if (string.IsNullOrEmpty(searchDirectory)) return null;

            var configPath = Path.Combine(searchDirectory, "buildtools.json");
            if (File.Exists(configPath))
            {
                return configPath;
            }
            else if (Path.IsPathRooted(searchDirectory))
            {
                GenerateDefaultConfig(searchDirectory);
                return searchDirectory;
            }

            return GetConfiguration(Directory.GetParent(searchDirectory).FullName);
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
            var outputDir = string.IsNullOrEmpty(SolutionDir) ? path : SolutionDir;
            Log.LogMessage($"Auto-Generating BuildTools configuration at '{outputDir}'");
            ConfigHelper.SaveDefaultConfig(outputDir);
        }
    }
}
