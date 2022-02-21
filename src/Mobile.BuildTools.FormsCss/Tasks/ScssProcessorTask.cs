using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.LibSass;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Tasks
{
    public class ScssProcessorTask : Microsoft.Build.Utilities.Task
    {
        private ILog _logger;
        internal ILog Logger
        {
            get => _logger ??= (BuildHostLoggingHelper)Log;
            set => _logger = value;
        }

        public string DebugOutput { get; set; }

        public string OutputDirectory { get; set; }

        public string[] ScssFiles { get; set; }

        private IEnumerable<ITaskItem> _generatedCssFiles;
        [Output]
        public ITaskItem[] GeneratedCssFiles => _generatedCssFiles?.ToArray() ?? new ITaskItem[0];

        public override bool Execute()
        {
            try
            {
//#if DEBUG
//                if (!Debugger.IsAttached && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
//                    Debugger.Launch();
//#endif

                var filesToProcess = ScssFiles.Where(scss => Path.GetFileName(scss)[0] != '_' && Path.GetExtension(scss) == ".scss" || Path.GetExtension(scss) == ".sass");
                _generatedCssFiles = ProcessFiles(filesToProcess);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("An Error occurred while processing the Sass files");
                Logger.LogErrorFromException(ex);
            }

            return !Log.HasLoggedErrors;
        }

        private IEnumerable<TaskItem> ProcessFiles(IEnumerable<string> inputFiles)
        {
            foreach (var file in inputFiles)
            {
                if (Path.GetFileName(file)[0] == '_')
                {
                    Logger.LogMessage($"Skipping Partial File: {file}");
                    continue;
                }

                Logger.LogMessage($"Processing {file}");
                var outputFile = GetFilePath(file);

                var options = new ScssOptions
                {
                    InputFile = file,
                    OutputStyle = ScssOutputStyle.Compressed,
                    GenerateSourceMap = false,
                };
                var scssContents = File.ReadAllText(file);
                var result = Scss.ConvertToCss(scssContents, options);

                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                Logger.LogMessage($"Finished processing '{file}'");
                var css = result.Css;
                Logger.LogMessage(css);

                if (string.IsNullOrWhiteSpace(DebugOutput) || (bool.TryParse(DebugOutput, out var b) && !b))
                {
                    var debugOptions = new ScssOptions
                    {
                        InputFile = file,
                        Linefeed = Environment.NewLine,
                        SourceComments = true,
                        OutputStyle = ScssOutputStyle.Expanded
                    };
                    Logger.LogMessage(Scss.ConvertToCss(scssContents, debugOptions).Css);
                }

                // HACK: ^ selector is not valid CSS/Sass. This replaces alternate valid syntax with the ^ selector for Xamarin Forms
                var pattern = @"(\w+):(any|all)";
                var formsCSS = Regex.Replace(css, pattern, m => $"^{m.Groups[1].Value}");

                File.WriteAllText(outputFile, $"{formsCSS}");
                yield return new TaskItem(ProjectCollection.Escape(outputFile));
            }
        }

        private string GetFilePath(string scssFile)
        {
            var file = Regex.Replace(scssFile, @"sass|scss", "css");
            return Path.Combine(OutputDirectory, file);
        }
    }
}
