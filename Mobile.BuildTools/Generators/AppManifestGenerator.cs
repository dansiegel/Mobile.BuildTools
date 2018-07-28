using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Generators
{
    public class AppManifestGenerator : IGenerator
    {
        internal const string DefaultToken = @"\$\$";

        public string Prefix { get; set; }

        public string Token { get; set; }

        public string SdkShortFrameworkIdentifier { get; set; }

        public string ProjectDirectory { get; set; }

        public string ManifestTemplatePath { get; set; }

        public string ManifestOutputPath { get; set; }

        public bool? DebugOutput { get; set; }

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

            if(DebugOutput == null)
            {
                DebugOutput = false;
            }

            var template = File.ReadAllText(ManifestTemplatePath);
            var variables = Utils.EnvironmentAnalyzer.GatherEnvironmentVariables(ProjectDirectory, true);
            foreach (Match match in GetMatches(template))
            {
                template = ProcessMatch(template, match, variables);
            }

            WriteManifest(template);
        }

        internal MatchCollection GetMatches(string template)
        {
            var pattern = $@"{Token}(.*?){Token}";
            return Regex.Matches(template, pattern);
        }

        internal string ProcessMatch(string template, Match match, IDictionary<string, string> variables)
        {
            var tokenId = match.Groups[1].Value;
            var key = GetKey(tokenId, variables);
            if (!string.IsNullOrWhiteSpace(key))
            {
                Log?.LogMessage($"Replacing token '{tokenId}'");
                var value = variables[key];
                template = Regex.Replace(template, $@"{Token}{tokenId}{Token}", value);
            }
            else
            {
                Log?.LogWarning($"Unable to locate replacement value for {tokenId}");
            }

            return template;
        }

        internal string GetKey(string matchValue, IDictionary<string, string> variables)
        {
            if(variables.ContainsKey(matchValue))
            {
                return matchValue;
            }

            if (variables.ContainsKey($"{Prefix}{matchValue}"))
            {
                return $"{Prefix}{matchValue}";
            }

            foreach(var manifestPrefix in Utils.EnvironmentAnalyzer.GetManifestPrefixes(SdkShortFrameworkIdentifier))
            {
                if (variables.ContainsKey($"{manifestPrefix}{matchValue}"))
                    return $"{manifestPrefix}{matchValue}";
            }

            return null;
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