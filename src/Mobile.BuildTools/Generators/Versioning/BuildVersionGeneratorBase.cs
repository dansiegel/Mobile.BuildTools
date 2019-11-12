using System;
using System.IO;
using System.Linq;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Utils;
using Mobile.BuildTools.Versioning;

namespace Mobile.BuildTools.Generators.Versioning
{
    public abstract class BuildVersionGeneratorBase : IGenerator
    {
        private static DateTimeOffset EPOCOffset => new DateTimeOffset(new DateTime(2018, 1, 1));

        public string ManifestPath { get; set; }
        public Behavior Behavior { get; set; }
        public VersionEnvironment VersionEnvironment { get; set; }
        public int VersionOffset { get; set; }
        public ILog Log { get; set; }
        public bool? DebugOutput { get; set; }

        internal string BuildNumber { get; private set; }

        public void Execute()
        {
            if (DebugOutput == null)
            {
                DebugOutput = false;
            }

            if(Behavior == Behavior.Off)
            {
                Log.LogMessage("Automatic versioning has been disabled. Please update your project settings to enable automatic versioning.");
                return;
            }

            if(string.IsNullOrWhiteSpace(ManifestPath))
            {
                Log.LogMessage("This platform is unsupported");
                return;
            }

            if(!File.Exists(ManifestPath))
            {
                Log.LogWarning($"The '{Path.GetFileName(ManifestPath)}' could not be found at the path '{ManifestPath}'.");
                return;
            }

            BuildNumber = GetBuildNumber();
            Log.LogMessage($"Build Number: {BuildNumber}");

            LogManifestContents();

            Log.LogMessage("Processing Manifest");
            ProcessManifest(ManifestPath, BuildNumber);

            LogManifestContents();
        }

        private void LogManifestContents()
        {
            if (DebugOutput.HasValue && DebugOutput.Value)
            {
                Log.LogMessage(File.ReadAllText(ManifestPath));
            }
        }

        protected abstract void ProcessManifest(string path, string buildNumber);

        protected string GetBuildNumber()
        {
            if(Behavior == Behavior.PreferBuildNumber && CIBuildEnvironmentUtils.IsBuildHost)
            {
                if(int.TryParse(CIBuildEnvironmentUtils.BuildNumber, out var buildId))
                {
                    return $"{buildId + VersionOffset}";
                }

                return CIBuildEnvironmentUtils.BuildNumber;
            }

            var timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds() - EPOCOffset.ToUnixTimeSeconds();
            return $"{VersionOffset + timeStamp}";
        }

        protected string SanitizeVersion(string version)
        {
            try
            {
                var parts = version.Split('.');
                return parts.LastOrDefault().Count() > 4 ?
                    string.Join(".", parts.Where(p => p != parts.LastOrDefault())) :
                    version;
            }
            catch(Exception e)
            {
                Log.LogMessage($"Error Sanitizing Version String");
                Log.LogWarning(e.ToString());
                return version;
            }
        }
    }
}
