using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Tasks
{
    public class SecretsJsonTask : Microsoft.Build.Utilities.Task
    {
        private ITaskItem[] _generatedCodeFiles;

        public string Configuration { get; set; }

        public string ProjectBasePath { get; set; }

        public string SecretsClassName { get; set; }

        public string SecretsJsonFilePath { get; set; }

        public string RootNamespace { get; set; }

        public string OutputPath { get; set; }

        public string IntermediateOutputPath { get; set; }

        public string DebugOutput { get; set; }

        [Output]
        public ITaskItem[] GeneratedCodeFiles => _generatedCodeFiles ?? new ITaskItem[0];

        public override bool Execute()
        {
            if (!ValidateParameters()) return false;

            try
            {
                var configurationSecretsFilePath = $"{Path.GetFileNameWithoutExtension(SecretsJsonFilePath)}.{Configuration.ToLower()}{Path.GetExtension(SecretsJsonFilePath)}";
                if (File.Exists(SecretsJsonFilePath) || File.Exists(configurationSecretsFilePath))
                {
                    Log.LogMessage($"ProjectBasePath: {ProjectBasePath}");
                    Log.LogMessage($"SecretsClassName: {SecretsClassName}");
                    Log.LogMessage($"SecretsJsonFilePath: {SecretsJsonFilePath}");
                    Log.LogMessage($"RootNamespace: {RootNamespace}");
                    Log.LogMessage($"OutputPath: {OutputPath}");

                    bool.TryParse(DebugOutput, out var debug);
                    var generator = new SecretsClassGenerator()
                    {
                        ConfigurationSecretsJsonFilePath = configurationSecretsFilePath,
                        ProjectBasePath = ProjectBasePath,
                        SecretsClassName = SecretsClassName,
                        SecretsJsonFilePath = SecretsJsonFilePath,
                        BaseNamespace = RootNamespace,
                        OutputPath = OutputPath,
                        IntermediateOutputPath = IntermediateOutputPath,
                        DebugOutput = debug,
                        Log = (BuildHostLoggingHelper)Log
                    };
                    generator.Execute();
                    _generatedCodeFiles = generator.GeneratedFiles;
                }
                else
                {
                    Log.LogMessage("No Secrets Json File was found.");
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.ToString());
                return false;
            }

            return true;
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