using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Tasks.Utils;

namespace Mobile.BuildTools.Tasks
{
    public class CollectImageAssetsTask : BuildToolsTaskBase
    {
        public string AdditionalSearchPaths { get; set; }

        [Output]
        public ITaskItem[] GeneratedImages { get; private set; }

        [Output]
        public ITaskItem[] SourceImages { get; private set; }

        [Output]
        public bool HasImages => GeneratedImages.Length > 0;

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Launch();
            else
                System.Diagnostics.Debugger.Break();
#endif

            GeneratedImages = Array.Empty<ITaskItem>();
            var generator = CreateGenerator(config.Platform, config);
            if(generator is null)
            {
                Log.LogWarning($"Cannot collect image assets for {TargetFrameworkIdentifier}, target framework is not supported.");
                return;
            }

            generator.Execute();
            SourceImages = generator.ImageInputFiles.Select(x => new TaskItem(x)).ToArray();
            GeneratedImages = generator.Outputs.Select(x => x.ToTaskItem()).ToArray();
        }

        private ImageCollectionGeneratorBase CreateGenerator(Platform platform, IBuildConfiguration config)
        {
            switch(platform)
            {
                case Platform.Android:
                    return new AndroidImageCollectionGenerator(this)
                    {
                        SearchFolders = GetSearchPaths(config)
                    };
                case Platform.iOS:
                case Platform.macOS:
                    return new AppleImageCollectionGenerator(this)
                    {
                        SearchFolders = GetSearchPaths(config)
                    };
                default: return null;
            }
        }

        private IEnumerable<string> GetSearchPaths(IBuildConfiguration config)
        {
            var searchPaths = new List<string>();
            var imageConfig = config.Configuration.Images;
            if(imageConfig.Directories?.Any() ?? false)
            {
                searchPaths.AddRange(imageConfig.Directories.Select(GetSearchPath));
            }

            if(AdditionalSearchPaths?.Split(';')?.Any() ?? false)
            {
                searchPaths.AddRange(AdditionalSearchPaths.Split(';').Select(GetSearchPath));
            }

            var monoandroidKey = imageConfig.ConditionalDirectories.Keys.FirstOrDefault(x => x.Equals("monoandroid", StringComparison.InvariantCultureIgnoreCase));
            var xamariniOSKey = imageConfig.ConditionalDirectories.Keys.FirstOrDefault(x => x.Equals("xamarin.ios", StringComparison.InvariantCultureIgnoreCase) || x.Equals("xamarinios", StringComparison.InvariantCultureIgnoreCase));
            var xamarinMacKey = imageConfig.ConditionalDirectories.Keys.FirstOrDefault(x => x.Equals("xamarin.mac", StringComparison.InvariantCultureIgnoreCase) || x.Equals("xamarinmac", StringComparison.InvariantCultureIgnoreCase));

            switch(config.Platform)
            {
                case Platform.Android:
                    if (!string.IsNullOrEmpty(monoandroidKey))
                        searchPaths.AddRange(imageConfig.ConditionalDirectories[monoandroidKey].Select(GetSearchPath));
                    break;
                case Platform.iOS:
                    if (!string.IsNullOrEmpty(xamariniOSKey))
                        searchPaths.AddRange(imageConfig.ConditionalDirectories[xamariniOSKey].Select(GetSearchPath));
                    break;
                case Platform.macOS:
                    if (!string.IsNullOrEmpty(xamarinMacKey))
                        searchPaths.AddRange(imageConfig.ConditionalDirectories[xamarinMacKey].Select(GetSearchPath));
                    break;
            }

            // TODO: Make this even smarter with conditions like `Release || Store`... perhaps we also should consider evaluating the defined constants.
            var keys = imageConfig.ConditionalDirectories.Keys.Where(k => !k.StartsWith("mono") && !k.StartsWith("xamarin") && (k.Equals(config.BuildConfiguration) || !k.Equals($"!{config.BuildConfiguration}")));
            foreach(var validCondition in keys)
            {
                searchPaths.AddRange(imageConfig.ConditionalDirectories[validCondition].Select(GetSearchPath));
            }

            return searchPaths.Distinct();
        }

        private string GetSearchPath(string directory)
        {
            if (Uri.TryCreate(directory, UriKind.RelativeOrAbsolute, out var result) && result.IsAbsoluteUri)
                return directory;

            return Path.Combine(ConfigurationPath, directory);
        }
    }
}
