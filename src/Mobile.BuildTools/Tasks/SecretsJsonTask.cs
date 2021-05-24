using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Secrets;
using Mobile.BuildTools.Utils;

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
            //System.Diagnostics.Debugger.Launch();

            var secretsConfig = config.GetSecretsConfig();
            if (secretsConfig is null || secretsConfig.Disable)
                return;

            var configJson = string.Format(Constants.SecretsJsonConfigurationFileFormat, config.BuildConfiguration.ToLower());
            var searchPaths = new[]
            {
                SolutionDirectory,
                ProjectDirectory,
                ConfigHelper.GetConfigurationPath(ProjectDirectory, SolutionDirectory)
            }
            .SelectMany(x => new[] { Path.Combine(x, Constants.SecretsJsonFileName), Path.Combine(x, configJson) })
            .Where(x => File.Exists(x))
            .ToList();

            if (!string.IsNullOrEmpty(JsonSecretsFilePath) && !searchPaths.Contains(JsonSecretsFilePath))
                searchPaths.Add(JsonSecretsFilePath);

            var rootNamespace = string.IsNullOrEmpty(secretsConfig.RootNamespace) ? RootNamespace : secretsConfig.RootNamespace;
            var generator = new SecretsClassGenerator(config, searchPaths.ToArray())
            {
                BaseNamespace = rootNamespace,
            };
            generator.Execute();
            _generatedCodeFiles = new[] { generator.Outputs };
        }
    }
}
