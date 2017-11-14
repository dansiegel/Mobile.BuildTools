using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;

namespace Mobile.BuildTools.Generators
{
    public class AppManifestGenerator
    {
        public string Prefix { get; set; }

        public string Token { get; set; }

        public string ManifestTemplatePath { get; set; }

        public string ManifestOutputPath { get; set; }

        public TaskLoggingHelper Log { get; set; }

        public void Execute()
        {
            if(string.IsNullOrWhiteSpace(Token))
            {
                Token = @"\$\$";
            }

            var pattern = $@"{Token}(.*?){Token}";
            var template = File.ReadAllText(ManifestTemplatePath);
            var matches = Regex.Matches(template, pattern);

            foreach(Match match in matches)
            {
                var key = $"{Prefix}{match.Groups[1]}";
                if(Environment.GetEnvironmentVariables().Contains(key))
                {
                    Log.LogMessage($"Replacing token '{key}'");
                    var value = Environment.GetEnvironmentVariable(key);
                    template = Regex.Replace(template, $@"{Token}{key}{Token}", value);
                }
                else
                {
                    Log.LogWarning($"Unable to locate replacement value for {key}");
                }
            }

            File.WriteAllText(ManifestOutputPath, template);
        }
    }
}