using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Json;
using Microsoft.Build.Utilities;

namespace Mobile.BuildTools.Generators
{
    public class BuildHostSecretsGenerator
    {
        public string SecretsPrefix { get; set; }

        public string SecretsJsonFilePath { get; set; }

        public void Execute()
        {
            var keys = Environment.GetEnvironmentVariables()
                                  .Keys
                                  .Cast<string>()
                                  .Where(k => k.StartsWith(SecretsPrefix));
            if(!keys.Any()) return;

            var json = new JsonObject();
            foreach(var key in keys)
                json.Add(key, Environment.GetEnvironmentVariable(key));

            using(var stream = File.Create(SecretsJsonFilePath))
            {
                json.Save(stream);
            }
        }
    }
}