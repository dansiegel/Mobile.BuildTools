using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.LibSass;

namespace Mobile.BuildTools.Generators
{
    internal class ScssGenerator : GeneratorBase<IEnumerable<string>>
    {
        public ScssGenerator(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        public IEnumerable<string> InputFiles { get; set; }

        protected override void ExecuteInternal()
        {
            var files = new List<string>();
            foreach (var file in InputFiles.Where(scss => Path.GetFileName(scss)[0] != '_' && Path.GetExtension(scss) == ".scss" || Path.GetExtension(scss) == ".sass"))
            {
                if (Path.GetFileName(file)[0] == '_')
                {
                    Log.LogMessage($"Skipping Partial File: {file}");
                    continue;
                }

                Log.LogMessage($"Processing {file}");
                var outputFile = GetFilePath(file);

                var options = new ScssOptions
                {
                    InputFile = file,
                    OutputStyle = Build.Configuration.Css?.Minify ?? true ? ScssOutputStyle.Compressed : ScssOutputStyle.Expanded,
                    GenerateSourceMap = false,
                };
                var scssContents = File.ReadAllText(file);
                var result = Scss.ConvertToCss(scssContents, options);

                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                Log.LogMessage($"Finished processing '{file}'");
                var css = result.Css;
                Log.LogMessage(css);

                if (Build.Configuration.Debug)
                {
                    var debugOptions = new ScssOptions
                    {
                        InputFile = file,
                        Linefeed = Environment.NewLine,
                        SourceComments = true,
                        OutputStyle = ScssOutputStyle.Expanded
                    };
                    Log.LogMessage(Scss.ConvertToCss(scssContents, debugOptions).Css);
                }

                // HACK: ^ selector is not valid CSS/Sass. This replaces alternate valid syntax with the ^ selector for Xamarin Forms
                var pattern = @"(\w+):(any|all)";
                var formsCSS = Regex.Replace(css, pattern, m => $"^{m.Groups[1].Value}");

                File.WriteAllText(outputFile, $"{formsCSS}");
                files.Add(outputFile);
            }
            Outputs = files;
        }

        private string GetFilePath(string scssFile)
        {
            var file = Regex.Replace(scssFile, @"sass|scss", "css");
            return Path.Combine(Build.IntermediateOutputPath, file);
        }
    }
}
