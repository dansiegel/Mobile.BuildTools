using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Models.AppIcons;
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
            var generator = CreateGenerator(config.Platform);
            if(generator is null)
            {
                GeneratedImages = Array.Empty<ITaskItem>();
                return;
            }

            generator.Execute();
            GeneratedImages = generator.Outputs.Select(x => x.ToTaskItem()).ToArray();
        }

        private ImageCollectionGeneratorBase CreateGenerator(Platform platform)
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
