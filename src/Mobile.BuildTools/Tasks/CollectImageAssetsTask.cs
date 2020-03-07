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

        public bool? IgnoreDefaultSearchPaths { get; set; }

        [Output]
        public ITaskItem[] GeneratedImages { get; private set; }

        [Output]
        public ITaskItem[] SourceImages { get; private set; }

        [Output]
        public bool HasImages => GeneratedImages.Length > 0;

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
// #if DEBUG
//             if (!System.Diagnostics.Debugger.IsAttached)
//                 System.Diagnostics.Debugger.Launch();
//             else
//                 System.Diagnostics.Debugger.Break();
// #endif

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
            var cliSearchPaths = !string.IsNullOrEmpty(AdditionalSearchPaths) ? AdditionalSearchPaths.Split(';') : Array.Empty<string>();
            if (cliSearchPaths.Any())
            {
                searchPaths.AddRange(AdditionalSearchPaths.Split(';').Select(GetSearchPath));
            }

            if(cliSearchPaths.Any() && IgnoreDefaultSearchPaths.HasValue && IgnoreDefaultSearchPaths.Value)
            {
                return searchPaths.Distinct();
            }


            if (imageConfig.Directories?.Any() ?? false)
            {
                searchPaths.AddRange(imageConfig.Directories.Select(GetSearchPath));
            }

            var monoandroidKey = GetKey(imageConfig.ConditionalDirectories.Keys, "monoandroid", "android", "droid");
            var xamariniOSKey = GetKey(imageConfig.ConditionalDirectories.Keys, "xamarin.ios", "xamarinios", "ios", "apple");
            var xamarinMacKey = GetKey(imageConfig.ConditionalDirectories.Keys, "xamarin.mac", "xamarinmac", "mac", "apple");
            var xamarinTVOSKey = GetKey(imageConfig.ConditionalDirectories.Keys, "xamarin.tvos", "xamarintvos", "tvos", "apple");

            var platformKeys = new[] { monoandroidKey, xamariniOSKey, xamarinMacKey, xamarinTVOSKey }.Where(x => x != null);

            switch (config.Platform)
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
                case Platform.TVOS:
                    if (!string.IsNullOrEmpty(xamarinTVOSKey))
                        searchPaths.AddRange(imageConfig.ConditionalDirectories[xamarinTVOSKey].Select(GetSearchPath));
                    break;
            }

            // TODO: Make this even smarter with conditions like `Release || Store`... perhaps we also should consider evaluating the defined constants.
            var keys = imageConfig.ConditionalDirectories.Keys.Where(k => !platformKeys.Any(x => x == k) && (k.Equals(config.BuildConfiguration) || !k.Equals($"!{config.BuildConfiguration}")));
            foreach(var validCondition in keys)
            {
                searchPaths.AddRange(imageConfig.ConditionalDirectories[validCondition].Select(GetSearchPath));
            }

            return searchPaths.Distinct();
        }

        private string GetKey(IEnumerable<string> conditionalKeys, params string[] possibleNames)
        {
            foreach(var name in possibleNames)
            {
                if (conditionalKeys.Any(x => x.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                    return name;
            }

            return null;
        }

        private string GetSearchPath(string directory)
        {
            if (Uri.TryCreate(directory, UriKind.RelativeOrAbsolute, out var result) && result.IsAbsoluteUri)
                return directory;

            return Path.Combine(ConfigurationPath, directory);
        }
    }
}
