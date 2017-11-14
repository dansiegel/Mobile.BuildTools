using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Json;

namespace Mobile.BuildTools.Generators
{
    public class BuildHostSecretsGenerator
    {
        public string SecretsPrefix { get; set; }

        public string SecretsJsonFilePath { get; set; }

        public void Execute()
        {
            var secrets = Environment.GetEnvironmentVariables()
                                  .Keys
                                  .Cast<DictionaryEntry>()
                                  .Where(e => $"{e.Key}".StartsWith(SecretsPrefix, StringComparison.InvariantCulture));
            if (!secrets.Any()) return;

            var json = new JsonObject();
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

            using (var stream = File.Create(SecretsJsonFilePath))
            {
                json.Save(stream);
            }
        }
    }
}