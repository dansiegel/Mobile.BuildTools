using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
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
            var secrets = GetSecrets();

            if (!secrets.Any())
            {
                Log?.LogMessage("    No Build Host Secrets Found...");
                return;
            }

            Log?.LogMessage($"Generating {Path.GetFileName(SecretsJsonFilePath)} for {SecretsPrefix}");
            var json = GetJObjectFromSecrets(secrets);

            File.WriteAllText(SecretsJsonFilePath,
                              json.ToString(Formatting.Indented));

            Log?.LogMessage(File.ReadAllText(SecretsJsonFilePath));
        }

        internal JObject GetJObjectFromSecrets(IEnumerable<object> secrets)
        {
            var json = new JObject();
            foreach (var secret in secrets)
            {
                var key = secret.ToString().Remove(0, SecretsPrefix.Length);
                var value = Environment.GetEnvironmentVariable(secret.ToString());
                if (double.TryParse(value, out double d))
                {
                    json.Add(key, Math.Abs(d % 1) <= (double.Epsilon * 100) ? (int)d : d);
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
            return json;
        }

        internal IEnumerable<object> GetSecrets() =>
            Environment.GetEnvironmentVariables()
                           .Keys
                           .Cast<object>()
                           .Where(e => $"{e}".StartsWith(SecretsPrefix, StringComparison.OrdinalIgnoreCase));

    }
}