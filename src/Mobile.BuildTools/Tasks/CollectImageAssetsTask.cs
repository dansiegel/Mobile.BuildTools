using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Tasks.Utils;

namespace Mobile.BuildTools.Tasks
{
    public class CollectImageAssetsTask : BuildToolsTaskBase
    {
        public string SearchPaths { get; set; }

        [Output]
        public ITaskItem[] GeneratedImages { get; private set; }

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            var generator = CreateGenerator(config.Platform, config);
            if(generator is null)
            {
                GeneratedImages = Array.Empty<ITaskItem>();
                return;
            }

            generator.Execute();
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
                searchPaths.AddRange(imageConfig.Directories);
            }

            if(SearchPaths?.Split(';')?.Any() ?? false)
            {
                searchPaths.AddRange(SearchPaths.Split(';'));
            }

            var monoandroidKey = imageConfig.ConditionalDirectories.Keys.FirstOrDefault(x => x.Equals("monoandroid", StringComparison.InvariantCultureIgnoreCase));
            var xamariniOSKey = imageConfig.ConditionalDirectories.Keys.FirstOrDefault(x => x.Equals("xamarin.ios", StringComparison.InvariantCultureIgnoreCase) || x.Equals("xamarinios", StringComparison.InvariantCultureIgnoreCase));
            var xamarinMacKey = imageConfig.ConditionalDirectories.Keys.FirstOrDefault(x => x.Equals("xamarin.mac", StringComparison.InvariantCultureIgnoreCase) || x.Equals("xamarinmac", StringComparison.InvariantCultureIgnoreCase));

            switch(config.Platform)
            {
                case Platform.Android:
                    if (!string.IsNullOrEmpty(monoandroidKey))
                        searchPaths.AddRange(imageConfig.ConditionalDirectories[monoandroidKey]);
                    break;
                case Platform.iOS:
                    if (!string.IsNullOrEmpty(xamariniOSKey))
                        searchPaths.AddRange(imageConfig.ConditionalDirectories[xamariniOSKey]);
                    break;
                case Platform.macOS:
                    if (!string.IsNullOrEmpty(xamarinMacKey))
                        searchPaths.AddRange(imageConfig.ConditionalDirectories[xamarinMacKey]);
                    break;
            }

            // TODO: Make this even smarter with conditions like `Release || Store`... perhaps we also should consider evaluating the defined constants.
            var keys = imageConfig.ConditionalDirectories.Keys.Where(k => !k.StartsWith("mono") && !k.StartsWith("xamarin") && (k.Equals(config.BuildConfiguration) || !k.Equals($"!{config.BuildConfiguration}")));
            foreach(var validCondition in keys)
            {
                searchPaths.AddRange(imageConfig.ConditionalDirectories[validCondition]);
            }

            return searchPaths;
        }
    }
}
