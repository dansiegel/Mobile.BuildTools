using System.IO;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Secrets;

namespace Mobile.BuildTools.Tasks
{
    public class SecretsJsonTask : BuildToolsTaskBase
    {
        private ITaskItem[] _generatedCodeFiles;

        public string RootNamespace { get; set; }

        public string JsonSecretsFilePath { get; set; }

        [Output]
        public ITaskItem[] GeneratedCodeFiles => _generatedCodeFiles ?? new ITaskItem[0];

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            if (config.GetSecretsConfig().Disable)
                return;

            var configJson = string.Format(Constants.SecretsJsonConfigurationFileFormat, config.BuildConfiguration.ToLower());

            var generator = new SecretsClassGenerator(config, 
                Path.Combine(SolutionDirectory, Constants.SecretsJsonFileName),
                Path.Combine(SolutionDirectory, configJson),
                Path.Combine(ProjectDirectory, Constants.SecretsJsonFileName),
                Path.Combine(ProjectDirectory, configJson),
                JsonSecretsFilePath)
            {
                BaseNamespace = RootNamespace,
            };
            generator.Execute();
            _generatedCodeFiles = new[] { generator.Outputs };
        }
    }
}
