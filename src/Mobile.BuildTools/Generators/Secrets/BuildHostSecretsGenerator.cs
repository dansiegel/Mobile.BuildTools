using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mobile.BuildTools.Build;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mobile.BuildTools.Generators.Secrets
{
    internal class BuildHostSecretsGenerator : GeneratorBase
    {
        public BuildHostSecretsGenerator(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        public string SecretsJsonFilePath { get; set; }

        protected override void Execute()
        {
            var config = Build.GetSecretsConfig();
            var secrets = GetSecrets(config?.Prefix);

            if (!secrets.Any())
            {
                Log?.LogMessage("    No Build Host Secrets Found...");
                return;
            }

            Log?.LogMessage($"Generating {Path.GetFileName(SecretsJsonFilePath)}");
            var json = GetJObjectFromSecrets(secrets);

            WriteJsonFile(SecretsJsonFilePath, json.ToString(Formatting.Indented));

            Log?.LogMessage(Build.Configuration.Debug ? json.ToString(Formatting.Indented) : SanitizeJObject(json));
        }

        internal string SanitizeJObject(JObject jObject)
        {
            foreach(var pair in jObject)
            {
                jObject[pair.Key] = "*****";
            }

            return jObject.ToString(Formatting.Indented);
        }

        internal void WriteJsonFile(string path, string json)
        {
            var dirPath = Path.GetDirectoryName(path);
            Directory.CreateDirectory(dirPath);
            File.WriteAllText(path, json);
        }

        internal JObject GetJObjectFromSecrets(IDictionary<string, string> secrets)
        {
            var json = new JObject();
            foreach (var secret in secrets)
            {
                var key = secret.Key;
                var value = secret.Value;
                if (double.TryParse(value, out var d))
                {
                    json.Add(key, Math.Abs(d % 1) <= (double.Epsilon * 100) ? (int)d : d);
                }
                else if (bool.TryParse(value, out var b))
                {
                    json.Add(key, b);
                }
                else
                {
                    json.Add(key, value);
                }
            }
            return json;
        }

        internal IDictionary<string, string> GetSecrets(string knownPrefix) =>
            Utils.EnvironmentAnalyzer.GetSecrets(Build.Platform, knownPrefix);
    }
}