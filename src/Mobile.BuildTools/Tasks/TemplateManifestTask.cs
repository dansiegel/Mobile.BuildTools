using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Generators.Manifests;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks
{
    public class TemplateManifestTask : BuildToolsTaskBase
    {
        public string[] ReferenceAssemblyPaths { get; set; }

        public string ManifestPath { get; set; }

        public string OutputManifestPath { get; set; }

        [Output]
        public ITaskItem ProcessedManifest { get; private set; }

        [Output]
        public string PackageId { get; private set; }

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            if(string.IsNullOrEmpty(ManifestPath))
            {
                Log.LogWarning("No value was provided for the Manifest. Unable to process Manifest Tokens");
                return;
            }

            if(!File.Exists(ManifestPath))
            {
                Log.LogWarning($"Unable to process Manifest Tokens, no manifest was found at the path '{ManifestPath}'");
                return;
            }

            if (string.IsNullOrEmpty(OutputManifestPath))
            {
                OutputManifestPath = ManifestPath;
            }

            BaseTemplatedManifestGenerator generator = null;
            switch(config.Platform)
            {
                case Platform.iOS:
                case Platform.macOS:
                case Platform.TVOS:
                    generator = new TemplatedPlistGenerator(config)
                    {
                        ManifestInputPath = ManifestPath,
                        ManifestOutputPath = OutputManifestPath
                    };
                    break;
                case Platform.Android:
                    generator = new TemplatedAndroidAppManifestGenerator(config, ReferenceAssemblyPaths)
                    {
                        ManifestInputPath = ManifestPath,
                        ManifestOutputPath = OutputManifestPath
                    };
                    break;
            }

            generator?.Execute();

            if(File.Exists(generator.Outputs))
            {
                ProcessedManifest = new TaskItem(generator.Outputs);
            }

            PackageId = generator?.PackageId ?? string.Empty;
        }
    }
}
