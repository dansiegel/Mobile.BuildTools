using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models.AppIcons;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Reference
{
    public static class ImageCollector
    {
        public static IEnumerable<OutputImage> GetGeneratedResources(string buildToolsConfigPath, string projectDirectory, string buildConfiguration, string intermediateOutputDirectory, string targetFramework, ILog logger = null) =>
            GetGeneratedResources(buildToolsConfigPath, projectDirectory, buildConfiguration, intermediateOutputDirectory, targetFramework.GetTargetPlatform(), logger);

        // This needs the proper IntermediateOutputPath
        public static IEnumerable<OutputImage> GetGeneratedResources(string buildToolsConfigPath, string projectDirectory, string buildConfiguration, string intermediateOutputDirectory, Platform platform, ILog logger = null)
        {
            if(!File.Exists(buildToolsConfigPath))
            {
                return Array.Empty<OutputImage>();
            }

            var buildConfig = new BuildConfiguration(projectDirectory, intermediateOutputDirectory, platform, logger);
            ImageCollectionGeneratorBase generator = platform switch
            {
                Platform.Android => new AndroidImageCollectionGenerator(buildConfig),
                Platform.iOS => new AppleImageCollectionGenerator(buildConfig),
                Platform.macOS => new AppleImageCollectionGenerator(buildConfig),
                Platform.TVOS => new AppleImageCollectionGenerator(buildConfig),
                _ => null
            };

            if (generator is null)
                return Array.Empty<OutputImage>();

            var buildToolsConfig = ConfigHelper.GetConfig(buildToolsConfigPath);
            generator.SearchFolders = ImageSearchUtil.GetSearchPaths(buildToolsConfig, platform, buildConfiguration, buildToolsConfigPath);
            generator.Execute();
            return generator.Outputs;
        }
    }
}
