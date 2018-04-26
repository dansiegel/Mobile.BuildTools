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
        internal const string UpgradeNote = @"/*
 * Note: Mobile.BuildTools will be changing the default output behavior in a
 * future release. Future releases will minimize the output, and place it in the 
 * intermediate output folder (obj). Please delete this file after updating the
 * Mobile.BuildTools package.
 */
";
        private ILog _logger;
        internal ILog Logger
        {
            get => _logger ?? (BuildHostLoggingHelper)Log;
            set => _logger = value;
        }

        public bool? OutputInProject { get; set; }

        public bool? MinimizeCSS { get; set; }

        public string OutputDirectory { get; set; }

        public string[] NoneIncluded { get; set; }

        private IEnumerable<ITaskItem> _generatedCssFiles;
        [Output]
        public ITaskItem[] GeneratedCssFiles => _generatedCssFiles.ToArray();

        public override bool Execute()
        {
            try
            {
                // HACK: Output in obj folder generates wrong resource id
                if (!OutputInProject.HasValue)
                    OutputInProject = true;
                _generatedCssFiles = ProcessFiles(GetFilesToProcess());
            }
            catch (Exception ex)
            {
                Logger.LogMessage("An Error occurred while processing the Sass files");
                Logger.LogMessage(ex.ToString());
                return false;
            }

            return true;
        }

        internal IEnumerable<string> GetFilesToProcess()
        {
            return from f in NoneIncluded
                   where (Path.GetExtension(f) == ".scss" ||
                          Path.GetExtension(f) == ".sass") &&
                         !Path.GetFileNameWithoutExtension(f)
                    .StartsWith("_", StringComparison.InvariantCulture)
                   select f;
        }

        private IEnumerable<TaskItem> ProcessFiles(IEnumerable<string> inputFiles)
        {
            foreach (var file in inputFiles)
            {
                Logger.LogMessage($"Processing {file}");
                var outputFile = GetFilePath(file);

                var options = new ScssOptions { InputFile = file };
                var result = Scss.ConvertToCss(File.ReadAllText(file), options);

                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                Logger.LogMessage(result.Css);
                CssSettings settings = new CssSettings
                {
                    CommentMode = CssComment.None
                };

                Logger.LogMessage($"Finished processing '{file}', minifying output to '{outputFile}'");
                var css = result.Css;

                if (MinimizeCSS.HasValue && MinimizeCSS.Value)
                {
                    UglifyResult uglifyResult = Uglify.Css(result.Css, settings);

                    css = uglifyResult.Code;

                    ThrowUglifyErrors(uglifyResult, outputFile);
                }

                // HACK: ^ selector is not valid CSS/Sass. This replaces alternate valid syntax with the ^ selector for Xamarin Forms
                var pattern = @"(\w+):(any|all)";
                var formsCSS = Regex.Replace(css, pattern, m => $"^{m.Groups[1].Value}");

                File.WriteAllText(outputFile, $"{UpgradeNote}{formsCSS}");
                yield return new TaskItem(ProjectCollection.Escape(outputFile));
            }
        }

        private string GetFilePath(string scssFile)
        {
            var file = Regex.Replace(scssFile, @"sass|scss", "css");

            if (OutputInProject.HasValue && !OutputInProject.Value)
            {
                file = Path.Combine(OutputDirectory, file);
            }

            return file;
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