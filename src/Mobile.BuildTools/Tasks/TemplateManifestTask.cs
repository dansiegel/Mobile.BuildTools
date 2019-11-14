using System;
using System.IO;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Generators.Manifests;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Tasks.Utils;

namespace Mobile.BuildTools.Tasks
{
    public class TemplateManifestTask : BuildToolsTaskBase
    {
        public string[] ReferenceAssemblyPaths { get; set; }

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            IGenerator generator = null;
            switch(config.Platform)
            {
                case Platform.iOS:
                    generator = new TemplatedPlistGenerator(config);
                    break;
                case Platform.Android:
                    new TemplatedAndroidAppManifestGenerator(config, ReferenceAssemblyPaths);
                    break;
            }

            generator?.Execute();
        }
    }
}