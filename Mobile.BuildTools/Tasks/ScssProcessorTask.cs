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

        public string OutputDirectory { get; set; }

        public string[] NoneIncluded { get; set; }

        private IEnumerable<ITaskItem> _generatedCssFiles;
        [Output]
        public ITaskItem[] GeneratedCssFiles => _generatedCssFiles.ToArray();

        public override bool Execute()
        {
            try
            {
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
                var outputFile = Path.Combine(OutputDirectory, Regex.Replace(file, @"sass|scss", "css"));

                var options = new ScssOptions { InputFile = file };
                var result = Scss.ConvertToCss(File.ReadAllText(file), options);

                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                Logger.LogMessage(result.Css);
                CssSettings settings = new CssSettings
                {
                    CommentMode = CssComment.None
                };

                Logger.LogMessage($"Finished processing '{file}', minifying output to '{outputFile}'");
                UglifyResult uglifyResult = Uglify.Css(result.Css, settings);

                string minified = uglifyResult.Code;

                ThrowUglifyErrors(uglifyResult, outputFile);

                File.WriteAllText(outputFile, minified);

                yield return new TaskItem(ProjectCollection.Escape(outputFile));
            }
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