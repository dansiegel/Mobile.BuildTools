using Microsoft.Build.Framework;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Generators.Versioning;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks
{
    public class AutomaticBuildVersioningTask : BuildToolsTaskBase
    {
        public string[] ReferenceAssemblyPaths { get; set; }

        [Required]
        public string ManifestPath { get; set; }

        public string OutputManifestPath { get; set; }

        internal override void ExecuteInternal(IBuildConfiguration buildConfiguration)
        {
            if (buildConfiguration.Configuration.AutomaticVersioning.Behavior == VersionBehavior.Off)
            {
                Log.LogMessage("Automatic versioning has been disabled.");
            }
            else if (string.IsNullOrEmpty(ManifestPath))
            {
                Log.LogWarning($"No manifest path has been provided for {buildConfiguration.Platform}");
            }
            else if (!IsSupported(buildConfiguration.Platform))
            {
                Log.LogMessage($"The current Platform '{buildConfiguration.Platform}' is not supported for Automatic Versioning");
            } 
            else if (CIBuildEnvironmentUtils.IsBuildHost && buildConfiguration.Configuration.AutomaticVersioning.Environment == VersionEnvironment.Local)
            {
                Log.LogMessage("Your current settings are to only build on your local machine, however it appears you are building on a Build Host.");
            }
            else
            {
                Log.LogMessage("Executing Automatic Version Generator");
                var generator = GetGenerator(buildConfiguration.Platform);

                if (generator is null)
                    Log.LogWarning($"We seem to be on a supported platform {buildConfiguration.Platform}, but we did not get a generator...");
                else
                    generator.Execute();
            }
        }

        private IGenerator GetGenerator(Platform platform)
        {
            return platform switch
            {
                Platform.Android => new AndroidAutomaticBuildVersionGenerator(this, ManifestPath, OutputManifestPath)
                {
                    ReferenceAssemblyPaths = ReferenceAssemblyPaths
                },
                Platform.iOS => new iOSAutomaticBuildVersionGenerator(this, ManifestPath, OutputManifestPath),
                Platform.macOS => new iOSAutomaticBuildVersionGenerator(this, ManifestPath, OutputManifestPath),
                Platform.TVOS => new iOSAutomaticBuildVersionGenerator(this, ManifestPath, OutputManifestPath),
                _ => null
            };
        }

        private bool IsSupported(Platform platform)
        {
            return platform switch
            {
                Platform.Android => true,
                Platform.iOS => true,
                Platform.macOS => true,
                Platform.TVOS => true,
                _ => false
            };
        }
    }
}
