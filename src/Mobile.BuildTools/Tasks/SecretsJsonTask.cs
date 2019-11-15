using System;
using System.IO;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Generators.Secrets;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Tasks
{
    public class SecretsJsonTask : BuildToolsTaskBase
    {
        private ITaskItem[] _generatedCodeFiles;

        public string RootNamespace { get; set; }

        [Output]
        public ITaskItem[] GeneratedCodeFiles => _generatedCodeFiles ?? new ITaskItem[0];

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            var standardSecrets = Path.Combine(ProjectDirectory, "secrets.json");
            var configSecrets = Path.Combine(ProjectDirectory, $"secrets.{config.BuildConfiguration.ToLower()}.json");
            if (File.Exists(standardSecrets) || File.Exists(configSecrets))
            {
                var generator = new SecretsClassGenerator(config)
                {
                    ConfigurationSecretsJsonFilePath = configSecrets,
                    SecretsJsonFilePath = standardSecrets,
                    BaseNamespace = RootNamespace,
                };
                generator.Execute();
                _generatedCodeFiles = new[] { generator.Outputs };
            }
            else
            {
                Log.LogMessage("No Secrets Json File was found.");
            }
        }
    }
}