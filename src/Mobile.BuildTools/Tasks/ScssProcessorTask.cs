using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Logging;
using SharpScss;
using NUglify;
using NUglify.Css;

namespace Mobile.BuildTools.Tasks
{
    public class ScssProcessorTask : Microsoft.Build.Utilities.Task
    {
        private ILog _logger;
        internal ILog Logger
        {
            get => _logger ?? (BuildHostLoggingHelper)Log;
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
                var filesToProcess = ScssFiles.Where(scss => Path.GetFileName(scss)[0] != '_' && Path.GetExtension(scss) == ".scss" || Path.GetExtension(scss) == ".sass");
                _generatedCssFiles = ProcessFiles(filesToProcess);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("An Error occurred while processing the Sass files");
                Logger.LogMessage(ex.ToString());
                return false;
            }

            return true;
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

                var options = new ScssOptions { InputFile = file };
                var result = Scss.ConvertToCss(File.ReadAllText(file), options);

                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                Logger.LogMessage($"Finished processing '{file}'");
                var css = result.Css;
                Logger.LogMessage(css);

                if (string.IsNullOrWhiteSpace(DebugOutput) || (bool.TryParse(DebugOutput, out bool b) && !b))
                {
                    Logger.LogMessage($"minifying output to '{outputFile}'");
                    CssSettings settings = new CssSettings
                    {
                        CommentMode = CssComment.None,
                        TermSemicolons = true,
                    };

                    UglifyResult uglifyResult = Uglify.Css(result.Css, settings);

                    css = uglifyResult.Code;

                    Logger.LogMessage(css);

                    ThrowUglifyErrors(uglifyResult, outputFile);
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

        private void ThrowUglifyErrors(UglifyResult result, string outputFilePath)
        {
            if (!result.HasErrors) return;

            var errorMessage = $"Encountered an unexpected error while minifying {outputFilePath}";

            foreach (var error in result.Errors)
            {
                errorMessage += $"\n{error}";
            }

            throw new Exception(errorMessage);
        }
    }
}