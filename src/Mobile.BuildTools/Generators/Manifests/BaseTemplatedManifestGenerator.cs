using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Mobile.BuildTools.Build;

namespace Mobile.BuildTools.Generators.Manifests
{
    internal abstract class BaseTemplatedManifestGenerator : GeneratorBase<string>
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

        public string ManifestInputPath { get; set; }

        public string ManifestOutputPath { get; set; }

        protected override void ExecuteInternal()
        {
            Outputs = ManifestOutputPath;

            if (!File.Exists(ManifestInputPath))
            {
                Log?.LogWarning("There is no Template Manifest at the path: '{0}'", ManifestInputPath);
                return;
            }

            var outputInfo = new FileInfo(ManifestOutputPath);
            if (!outputInfo.Directory.Exists)
                outputInfo.Directory.Create();

            var template = ReadManifest();

            var variables = Utils.EnvironmentAnalyzer.GatherEnvironmentVariables(Build, true);
            foreach (Match match in GetMatches(template))
            {
                template = ProcessMatch(template, match, variables);
            }

            if (variables.ContainsKey(Constants.AppPackageName))
            {
                Log.LogMessage($"Setting App Package Name to: {variables[Constants.AppPackageName]}");
                template = SetAppBundleId(template, variables[Constants.AppPackageName]);
            }

            SaveManifest(template);
        }

        public abstract string GetBundId();

        protected abstract string SetAppBundleId(string manifest, string packageName);

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
                var value = variables[key];
                var message = Build.Configuration.Debug ? $"Replacing token '{tokenId}' with '{value}'." : $"Replacing token '{tokenId}'.";
                Log?.LogMessage(message);
                template = Regex.Replace(template, $@"{token}{tokenId}{token}", value);
            }
            else
            {
                var errorMessage = $"Unable to locate replacement value for '{match.Value}' on line {GetLineNumber(template, match.Value)}";
                if (Build.Configuration.Manifests.MissingTokensAsErrors)
                {
                    Log?.LogError(errorMessage);
                }
                else
                {
                    Log?.LogWarning(errorMessage);
                }
            }

            return template;
        }

        private static int GetLineNumber(string input, string value)
        {
            var lines = input.Split('\n');
            for(var i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(value))
                    return i + 1;
            }

            return -1;
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
