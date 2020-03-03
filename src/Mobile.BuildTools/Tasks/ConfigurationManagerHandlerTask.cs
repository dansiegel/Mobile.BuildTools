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

        public ITaskItem[] ExpectedAppConfig { get; set; }

        [Required]
        public string AppConfigEnvironment { get; set; }

        private List<ITaskItem> _outputs = new List<ITaskItem>();
        [Output]
        public ITaskItem[] OutputConfigs => _outputs.ToArray();

        public ITaskItem[] GeneratedAppConfig => ExpectedAppConfig;

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            //if (!Debugger.IsAttached)
            //    Debugger.Launch();

            if (!InputConfigFiles.Any())
            {
                Log.LogMessage("No input config files were found");
                return;
            }

            //var configsOutputDirPath = Path.Combine(IntermediateOutputPath, "configs");
            //var configsOutputDir = new DirectoryInfo(configsOutputDirPath);

            var rootConfigFile = InputConfigFiles.FirstOrDefault(x => x.ItemSpec.Equals("app.config", StringComparison.InvariantCultureIgnoreCase));
            var transformFile = InputConfigFiles.FirstOrDefault(x => x.ItemSpec.Equals($"app.{AppConfigEnvironment}.config", StringComparison.InvariantCultureIgnoreCase));

            if(transformFile is null)
            {
                Log.LogMessage($"No transform config 'app.{AppConfigEnvironment.ToLower()}.config' could be found in the output directory.");
                return;
            }

            if(rootConfigFile is null)
            {
                // this should never happen...
                Log.LogError("We could not locate an 'app.config' in the output directory. Please file a bug.");
                return;
            }

            var generator = new ConfigurationManagerTransformationGenerator(config)
            {
                BaseConfigPath = rootConfigFile.ItemSpec,
                TransformFilePath = transformFile.ItemSpec
            };
            generator.Execute();

            _outputs.AddRange(generator.Outputs);
        }

        //private string PrimaryTaskItemPath(bool isSanityCheck = false)
        //{
        //    var item = InputConfigFiles.FirstOrDefault(x => x.GetMetadata("IsRootConfig").Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase));

        //    if(item is null)
        //    {
        //        item = InputConfigFiles.FirstOrDefault(x => Path.GetFileName(x.ItemSpec).Equals("app.config", StringComparison.InvariantCultureIgnoreCase));
        //    }

        //    if(item is null && !isSanityCheck)
        //    {
        //        var searchDirectories = InputConfigFiles.Select(x => new FileInfo(x.ItemSpec).Directory).Distinct();

        //        var inputFiles = new List<ITaskItem>();
        //        foreach(var directory in searchDirectories)
        //        {
        //            inputFiles.AddRange(directory.EnumerateFiles("*.config").Select(x => new TaskItem(x.FullName)));
        //        }
        //        InputConfigFiles = inputFiles.ToArray();
        //        return PrimaryTaskItemPath(true);
        //    }

        //    return item?.ItemSpec;
        //}

        //private IEnumerable<string> GetOutputFiles(string outputFileDirectory)
        //{
        //    //int files = InputConfigFiles.Length;
        //    var files = InputConfigFiles.Select(x => x.ItemSpec);
        //    foreach (var file in files)
        //    {
        //        var fi = new FileInfo(file);
        //        var outputPath = Path.Combine(outputFileDirectory, Path.GetFileName(file));
        //        fi.CopyTo(outputPath);
        //        yield return outputPath;
        //        //yield return CopyFile(file, outputFileDirectory);
        //    }
        //}

        //private string CopyFile(string inputFile, string outputFileDirectory)
        //{
        //    var inputFileInfo = new FileInfo(inputFile);
        //    var fileName = Path.GetFileName(inputFile);
        //    var outputFile = Path.Combine(outputFileDirectory, fileName);

        //    if(!File.Exists(outputFile) || File.GetLastWriteTime(outputFile) < inputFileInfo.LastWriteTime)
        //    {
        //        var contents = File.ReadAllText(inputFile);

        //        Log.LogMessage($"Copying File {inputFile} to {outputFile}");
        //        File.WriteAllText(outputFile, contents);
        //        return outputFile;
        //    }

        //    if(File.Exists(outputFile))
        //    {
        //        Log.LogMessage(MessageImportance.Low, $"No update necessary for {outputFile}");
        //    }
        //    else
        //    {
        //        Log.LogError($"An expected error occurrected while attempting to copy the file {inputFile} to {outputFile}");
        //    }

        //    return outputFile;
        //}
    }
}
