using System;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks
{
    public sealed class PrepareNativeImageAssetsTask : BuildToolsTaskBase
    {
        public ITaskItem[] Images { get; set; } = Array.Empty<ITaskItem>();
        public string RuntimeIdentifier { get; set; }
        public string AdditionalSearchPaths { get; set; }
        public bool IgnoreDefaultSearchPaths { get; set; }

        [Output]
        public ITaskItem[] PreparedImages { get; private set; } = Array.Empty<ITaskItem>();

        [Output]
        public ITaskItem[] FileWrites { get; private set; } = Array.Empty<ITaskItem>();

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            // Conditional folders override shared folders; explicitly supplied search paths take priority.
            var configuredPaths = ImageSearchUtil.GetSearchPaths(config.Configuration, config.Platform, config.BuildConfiguration,
                ConfigurationPath, null, null, config.Logger).Reverse();
            var explicitPaths = ImageSearchUtil.GetSearchPaths(config.Configuration, config.Platform, config.BuildConfiguration,
                ConfigurationPath, AdditionalSearchPaths, true, config.Logger);
            var paths = string.IsNullOrWhiteSpace(AdditionalSearchPaths) ? configuredPaths :
                IgnoreDefaultSearchPaths == true ? explicitPaths : explicitPaths.Concat(configuredPaths);
            var preparer = new NativeImagePreparer(config, TargetFrameworkIdentifier, RuntimeIdentifier, paths, ConfigurationPath);
            PreparedImages = Images.Select(preparer.Prepare).Where(x => x != null).ToArray();
            FileWrites = preparer.FileWrites.Distinct().Select(x => new TaskItem(x)).ToArray();
        }
    }
}

