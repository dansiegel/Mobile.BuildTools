using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Generators
{
    public class AppManifestGenerator
    {
        internal const string DefaultToken = @"\$\$";

        public string Prefix { get; set; }

        public string Token { get; set; }

        public string ManifestTemplatePath { get; set; }

        public string ManifestOutputPath { get; set; }

        public ILog Log { get; set; }

        public void Execute()
        {
            if (string.IsNullOrWhiteSpace(Token))
            {
                Token = DefaultToken;
            }

            if (!File.Exists(ManifestTemplatePath))
            {
                Log?.LogWarning("There is no Template Manifest at the path: '{0}'", ManifestTemplatePath);
            }

            var template = File.ReadAllText(ManifestTemplatePath);
            foreach (Match match in GetMatches(template))
            {
                template = ProcessMatch(template, match);
            }

            WriteManifest(template);
        }

        internal MatchCollection GetMatches(string template)
        {
            var pattern = $@"{Token}(.*?){Token}";
            return Regex.Matches(template, pattern);
        }

        internal string ProcessMatch(string template, Match match)
        {
            var key = $"{Prefix}{match.Groups[1]}";
            if (Environment.GetEnvironmentVariables().Contains(key))
            {
                Log?.LogMessage($"Replacing token '{key}'");
                var value = Environment.GetEnvironmentVariable(key);
                template = Regex.Replace(template, $@"{Token}{match.Groups[1]}{Token}", value);
            }
            else
            {
                Log?.LogWarning($"Unable to locate replacement value for {key}");
            }

            return template;
        }

        internal void WriteManifest(string template)
        {
            var dirPath = Path.GetDirectoryName(ManifestOutputPath);
            if (!string.IsNullOrWhiteSpace(dirPath))
                Directory.CreateDirectory(dirPath);
            File.WriteAllText(ManifestOutputPath, template);
        }
    }
}