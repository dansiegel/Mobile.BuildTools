using System;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks
{
    public sealed class PrepareLottieAssetsTask : BuildToolsTaskBase
    {
        public ITaskItem[] Assets { get; set; } = Array.Empty<ITaskItem>();
        public string RuntimeIdentifier { get; set; }
        public string AdditionalSearchPaths { get; set; }
        public bool IgnoreDefaultSearchPaths { get; set; }
        [Output] public ITaskItem[] PreparedAssets { get; private set; } = Array.Empty<ITaskItem>();
        [Output] public ITaskItem[] FileWrites { get; private set; } = Array.Empty<ITaskItem>();

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            var configured = ImageSearchUtil.GetSearchPaths(config.Configuration, config.Platform, config.BuildConfiguration,
                ConfigurationPath, null, null, config.Logger).Reverse();
            var additional = ImageSearchUtil.GetSearchPaths(config.Configuration, config.Platform, config.BuildConfiguration,
                ConfigurationPath, AdditionalSearchPaths, true, config.Logger);
            var paths = string.IsNullOrWhiteSpace(AdditionalSearchPaths) ? configured :
                IgnoreDefaultSearchPaths == true ? additional : additional.Concat(configured);
            var preparer = new LottieAssetPreparer(config, TargetFrameworkIdentifier, RuntimeIdentifier, paths, ConfigurationPath);
            PreparedAssets = preparer.Prepare(Assets);
            FileWrites = preparer.FileWrites.Select(path => new TaskItem(path)).ToArray();
        }
    }
}
