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

        public string Configuration { get; set; }

        public string ProjectBasePath { get; set; }

        public string SecretsClassName { get; set; }

        public string SecretsJsonFilePath { get; set; }

        public string RootNamespace { get; set; }

        public string OutputPath { get; set; }

        [Output]
        public ITaskItem[] GeneratedCodeFiles => _generatedCodeFiles ?? new ITaskItem[0];

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            var configurationSecretsFilePath = $"{Path.GetFileNameWithoutExtension(SecretsJsonFilePath)}.{Configuration.ToLower()}{Path.GetExtension(SecretsJsonFilePath)}";
            if (File.Exists(SecretsJsonFilePath) || File.Exists(configurationSecretsFilePath))
            {
                Log.LogMessage($"ProjectBasePath: {ProjectBasePath}");
                Log.LogMessage($"SecretsClassName: {SecretsClassName}");
                Log.LogMessage($"SecretsJsonFilePath: {SecretsJsonFilePath}");
                Log.LogMessage($"RootNamespace: {RootNamespace}");
                Log.LogMessage($"OutputPath: {OutputPath}");

                var generator = new SecretsClassGenerator(config)
                {
                    ConfigurationSecretsJsonFilePath = configurationSecretsFilePath,
                    SecretsClassName = SecretsClassName,
                    SecretsJsonFilePath = SecretsJsonFilePath,
                    BaseNamespace = RootNamespace,
                };
                ((IGenerator)generator).Execute();
                _generatedCodeFiles = generator.GeneratedFiles;
            }
            else
            {
                Log.LogMessage("No Secrets Json File was found.");
            }
        }

        private bool ValidateParameters()
        {
            if (!Directory.Exists(ProjectBasePath))
            {
                Log.LogError("The supplied Project Base Path does not exist");
                return false;
            }
            if (string.IsNullOrWhiteSpace(RootNamespace))
            {
                Log.LogError("There was no supplied Root Namespace");
                return false;
            }

            if (string.IsNullOrWhiteSpace(SecretsClassName))
            {
                SecretsClassName = "Secrets";
            }

            if (string.IsNullOrWhiteSpace(SecretsJsonFilePath))
            {
                SecretsJsonFilePath = Path.Combine(ProjectBasePath, "secrets.json");
            }

            if (string.IsNullOrWhiteSpace(OutputPath))
            {
                OutputPath = Path.Combine(ProjectBasePath, "Helpers");
            }

            if (!OutputPath.StartsWith(ProjectBasePath, StringComparison.OrdinalIgnoreCase))
            {
                OutputPath = Path.Combine(ProjectBasePath, OutputPath);
            }

            return true;
        }
    }
}