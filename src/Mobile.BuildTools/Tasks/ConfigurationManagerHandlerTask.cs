using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Tasks.Generators.AppConfig;

namespace Mobile.BuildTools.Tasks
{
    public class ConfigurationManagerHandlerTask : BuildToolsTaskBase
    {
        public ITaskItem[] InputConfigFiles { get; set; }

        [Required]
        public string AppConfigEnvironment { get; set; }

        private List<ITaskItem> _outputs = new List<ITaskItem>();
        [Output]
        public ITaskItem[] OutputConfigs => _outputs.ToArray();

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            if (!InputConfigFiles.Any())
            {
                Log.LogMessage("No input config files were found");
                return;
            }

            var badInputs = InputConfigFiles.Where(x => !x.ItemSpec.EndsWith(".config"));
            if(badInputs.Any())
            {
                foreach (var item in badInputs)
                    Log.LogError("Found invalid Config File input. Config files must end with the extension '.config': {0}", item.ItemSpec);
                return;
            }

            var primaryItemPath = PrimaryTaskItemPath();

            if(string.IsNullOrEmpty(primaryItemPath))
            {
                Log.LogError("Unable to determine which config is the root configuration. You must either mark one with the property IsRootConfig=\"true\" or have one named 'app.config'");
                return;
            }

            if(!File.Exists(primaryItemPath))
            {
                Log.LogError($"Unable to locate the specified app.config at '{primaryItemPath}'");
            }

            var primaryConfigFileName = Path.GetFileName(primaryItemPath);
            var configsOutputDir = Path.Combine(IntermediateOutputPath, "configs");
            if (!Directory.Exists(configsOutputDir))
            {
                Directory.CreateDirectory(configsOutputDir);
            }

            var transformFileName = $"{Path.GetFileNameWithoutExtension(primaryConfigFileName)}.{AppConfigEnvironment}.config";
            var outputFiles = InputConfigFiles.Select(item => CopyFile(item.ItemSpec, configsOutputDir));
            var transformFilePath = outputFiles
                    .FirstOrDefault(x => Path.GetFileName(x)
                                            .Equals(transformFileName, 
                                                    StringComparison.InvariantCultureIgnoreCase));

            if(Log.HasLoggedErrors)
            {
                return;
            }

            var generator = new ConfigurationManagerTransformationGenerator(config)
            {
                BaseConfigPath = Path.Combine(configsOutputDir, primaryConfigFileName),
                TransformFilePath = transformFilePath
            };
            generator.Execute();

            _outputs.AddRange(generator.Outputs);
        }

        private string PrimaryTaskItemPath(bool isSanityCheck = false)
        {
            var item = InputConfigFiles.FirstOrDefault(x => x.GetMetadata("IsRootConfig").Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase));

            if(item is null)
            {
                item = InputConfigFiles.FirstOrDefault(x => Path.GetFileName(x.ItemSpec).Equals("app.config", StringComparison.InvariantCultureIgnoreCase));
            }

            if(item is null && !isSanityCheck)
            {
                var searchDirectories = InputConfigFiles.Select(x => new FileInfo(x.ItemSpec).Directory).Distinct();

                var inputFiles = new List<ITaskItem>();
                foreach(var directory in searchDirectories)
                {
                    inputFiles.AddRange(directory.EnumerateFiles("*.config").Select(x => new TaskItem(x.FullName)));
                }
                InputConfigFiles = inputFiles.ToArray();
                return PrimaryTaskItemPath(true);
            }

            return item?.ItemSpec;
        }

        private string CopyFile(string inputFile, string outputFileDirectory)
        {
            var inputFileInfo = new FileInfo(inputFile);
            var fileName = Path.GetFileName(inputFile);
            var outputFile = Path.Combine(outputFileDirectory, fileName);

            if(!File.Exists(outputFile) || File.GetLastWriteTime(outputFile) < inputFileInfo.LastWriteTime)
            {
                var contents = File.ReadAllText(inputFile);

                Log.LogMessage($"Copying File {inputFile} to {outputFile}");
                File.WriteAllText(outputFile, contents);
            }

            if(File.Exists(outputFile))
            {
                Log.LogMessage(MessageImportance.Low, $"No update necessary for {outputFile}");
            }
            else
            {
                Log.LogError($"An expected error occurrected while attempting to copy the file {inputFile} to {outputFile}");
            }

            return outputFile;
        }
    }
}
