using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Logging;
using Xamarin.MacDev;

namespace Mobile.BuildTools.Generators.Manifests
{
    internal abstract class BaseTemplatedManifestGenerator : GeneratorBase
    {
        internal const string DefaultToken = @"\$\$";

        public BaseTemplatedManifestGenerator(IBuildConfiguration configuration)
            : base(configuration)
        {
            var token = configuration.Configuration.Manifests.Token;
            if (string.IsNullOrEmpty(token))
            {
                token = DefaultToken;
            }

            Token = token;
        }

        public string Token { get; }

        public string ProjectDirectory => Build.ProjectDirectory;

        public string ManifestOutputPath { get; set; }

        protected override void Execute()
        {
            if (!File.Exists(ManifestOutputPath))
            {
                Log?.LogWarning("There is no Template Manifest at the path: '{0}'", ManifestOutputPath);
            }

            var template = ReadManifest();

            var variables = Utils.EnvironmentAnalyzer.GatherEnvironmentVariables(ProjectDirectory, true);
            foreach (Match match in GetMatches(template))
            {
                template = ProcessMatch(template, match, variables);
            }

            SaveManifest(template);
        }

        protected abstract string ReadManifest();

        protected abstract void SaveManifest(string manifest);

        internal MatchCollection GetMatches(string template)
        {
            var token = Regex.Escape(Token);
            var pattern = $@"{token}(.*?){token}";
            return Regex.Matches(template, pattern);
        }

        internal string ProcessMatch(string template, Match match, IDictionary<string, string> variables)
        {
            var tokenId = match.Groups[1].Value;
            var key = GetKey(tokenId, variables);
            var token = Regex.Escape(Token);

            if (!string.IsNullOrWhiteSpace(key))
            {
                Log?.LogMessage($"Replacing token '{tokenId}'");
                var value = variables[key];
                template = Regex.Replace(template, $@"{token}{tokenId}{token}", value);
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

            var prefixes = Utils.EnvironmentAnalyzer.GetManifestPrefixes(Build.Platform, Build.Configuration.Manifests.VariablePrefix);

            foreach (var manifestPrefix in prefixes)
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