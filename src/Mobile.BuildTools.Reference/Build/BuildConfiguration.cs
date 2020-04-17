using System.IO;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Build
{
    internal class BuildConfiguration : IBuildConfiguration
    {
        public BuildConfiguration(string projectDirectory, string intermediateOutputDirectory, Platform platform, ILog logger)
        {
            ProjectDirectory = projectDirectory;
            IntermediateOutputPath = intermediateOutputDirectory;
            Platform = platform;
            Logger = logger ?? new ConsoleLogger();
        }

        public string ProjectDirectory { get; }
        public string IntermediateOutputPath { get; }
        public Platform Platform { get; }
        public ILog Logger { get; }
    }
}
