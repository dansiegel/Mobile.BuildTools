using System;
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Build.Utilities;
using Newtonsoft.Json.Linq;

namespace Mobile.BuildTools.Generators
{
    public class BuildHostSecretsGenerator
    {
        public string SecretsPrefix { get; set; }

        public string SecretsJsonFilePath { get; set; }

        public TaskLoggingHelper Log { get; set; }

        public void Execute()
        {
            var secrets = Environment.GetEnvironmentVariables()
                                     .Keys
                                     .Cast<DictionaryEntry>()
                                     .Where(e => $"{e.Key}".StartsWith(SecretsPrefix, StringComparison.OrdinalIgnoreCase));
            if (!secrets.Any())
            {
                Log.LogMessage("    No Build Host Secrets Found...");
                return;
            }

            Log.LogMessage($"Generating {Path.GetFileName(SecretsJsonFilePath)} for {SecretsPrefix}");
            var json = new JObject();
            foreach (var secret in secrets)
            {
                var key = secret.Key.ToString().Remove(0, SecretsPrefix.Length);
                var value = secret.Value.ToString();
                if (double.TryParse(value, out double d))
                {
                    json.Add(key, d % 1 == 0 ? (int)d : d);
                }
                else if (bool.TryParse(value, out bool b))
                {
                    json.Add(key, b);
                }
                else
                {
                    json.Add(key, value);
                }
            }

            File.WriteAllText(SecretsJsonFilePath,
                              json.ToString(Formatting.Indented));

            Log.LogMessage(File.ReadAllText(SecretsJsonFilePath));
        }
    }
}