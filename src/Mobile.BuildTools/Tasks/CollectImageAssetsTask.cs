using Microsoft.Build.Framework;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Tasks.Utils;

namespace Mobile.BuildTools.Tasks
{
    public class CollectImageAssetsTask : BuildToolsTaskBase
    {
        public ITaskItem Configuration { get; set; }

        public string TargetFramework { get; set; }

        public string SearchPaths { get; set; }

        [Output]
        public ITaskItem[] GeneratedImages { get; set; }

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            var generator = CreateGenerator(config.Platform, config);
            generator?.Execute();
        }

        private IGenerator CreateGenerator(Platform platform, IBuildConfiguration config)
        {
            switch(platform)
            {
                case Platform.Android:
                    return new AndroidImageCollectionGenerator(this);
                case Platform.iOS:
                case Platform.macOS:
                    return new AppleImageCollectionGenerator(this);
                default: return null;
            }
        }
    }
}
