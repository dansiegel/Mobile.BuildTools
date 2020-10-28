using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Handlers;
using Mobile.BuildTools.Models.Secrets;
using Mobile.BuildTools.Utils;
using Newtonsoft.Json.Linq;

namespace Mobile.BuildTools.Generators.Secrets
{
    internal class SecretsClassGenerator : GeneratorBase<ITaskItem>
    {
#pragma warning disable IDE1006, IDE0040
        private string CompileGeneratedAttribute { get; }

        private string[] SecretsFileLocations { get; }

        public SecretsClassGenerator(IBuildConfiguration buildConfiguration, params string[] secretsFileLocations)
            : base(buildConfiguration)
        {
            var assembly = typeof(SecretsClassGenerator).Assembly;
            var toolVersion = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
            CompileGeneratedAttribute = @$"[GeneratedCodeAttribute(""Mobile.BuildTools.Generators.Secrets.SecretsClassGenerator"", ""{toolVersion}"")]";

            SecretsFileLocations = secretsFileLocations;
        }
#pragma warning restore IDE1006, IDE0040

        public string BaseNamespace { get; set; }

        protected override void ExecuteInternal()
        {
            if (!SecretsFileLocations.Any(x => File.Exists(x)))
            {
                Log.LogMessage("No secrets.json was found in either the Project or Solution Directory.");
                return;
            }

            var secrets = GetMergedSecrets();

            var replacement = string.Empty;
            var safeReplacement = string.Empty;
            var secretsConfig = Build.GetSecretsConfig();
            var saveConfig = secretsConfig is null;
            if(saveConfig)
            {
                secretsConfig = new SecretsConfig()
                {
                    ClassName = "Secrets",
                    Namespace = "Helpers",
                    Delimiter = ";",
                    Prefix = "BuildTools_",
                    Properties = new List<ValueConfig>()
                };
            }

            if(saveConfig)
            {
                if (Build.Configuration.ProjectSecrets is null)
                {
                    Build.Configuration.ProjectSecrets = new Dictionary<string, SecretsConfig>();
                }
                Build.Configuration.ProjectSecrets.Add(Build.ProjectName, secretsConfig);
                Build.SaveConfiguration();
            }

            if(string.IsNullOrEmpty(secretsConfig.Namespace))
            {
                secretsConfig.Namespace = "Helpers";
            }

            if(string.IsNullOrEmpty(secretsConfig.ClassName))
            {
                secretsConfig.ClassName = "Secrets";
            }

            var writer = GenerateClass(secretsConfig, secrets);
            var secretsClass = writer.ToString();
            Log.LogMessage(Build.Configuration.Debug ? secretsClass : writer.SafeOutput);

            var namespacePath = string.Join($"{Path.PathSeparator}", secretsConfig.Namespace.Split('.').Where(x => !string.IsNullOrEmpty(x)));
            var fileName = $"{secretsConfig.ClassName}.g.cs";
            var projectFile = Path.Combine(namespacePath, fileName);
            var intermediateFile = Path.Combine(Build.IntermediateOutputPath, namespacePath, fileName);

            Log.LogMessage($"Writing Secrets Class to: '{intermediateFile}'");
            var generatedFile = new TaskItem(intermediateFile);
            generatedFile.SetMetadata("Visible", bool.TrueString);
            generatedFile.SetMetadata("Link", projectFile);
            if(File.Exists(projectFile))
            {
                generatedFile.SetMetadata("DependentUpon", $"{secretsConfig.ClassName}.cs");
            }

            Outputs = generatedFile;

            var parentDirectory = Directory.GetParent(intermediateFile);
            if(!Directory.Exists(parentDirectory.FullName))
            {
                Directory.CreateDirectory(parentDirectory.FullName);
            }

            File.WriteAllText(intermediateFile, secretsClass);
        }

        internal IDictionary<string, string> GetMergedSecrets()
        {
            JObject jObject = null;
            foreach (var file in SecretsFileLocations)
            {
                CreateOrMerge(file, ref jObject);
            }

            if (jObject is null)
            {
                throw new Exception("An unexpected error occurred. Could not locate any secrets.");
            }

            var secrets = jObject.ToObject<Dictionary<string, string>>();

            if (Build?.Configuration?.Environment != null)
            {
                var settings = Build.Configuration.Environment;
                var defaultSettings = settings.Defaults ?? new Dictionary<string, string>();
                if (settings.Configuration != null && settings.Configuration.ContainsKey(Build.BuildConfiguration))
                {
                    foreach ((var key, var value) in settings.Configuration[Build.BuildConfiguration])
                        defaultSettings[key] = value;
                }

                UpdateVariables(defaultSettings, ref secrets);
            }

            return secrets;
        }

        private static void UpdateVariables(IDictionary<string, string> settings, ref Dictionary<string, string> output)
        {
            if (settings is null)
                return;

            foreach ((var key, var value) in settings)
            {
                if (!output.ContainsKey(key))
                    output[key] = value;
            }
        }

        internal void CreateOrMerge(string jsonFilePath, ref JObject secrets)
        {
            if(!string.IsNullOrEmpty(jsonFilePath) && File.Exists(jsonFilePath))
            {
                var json = File.ReadAllText(jsonFilePath);
                if(secrets is null)
                {
                    secrets = JObject.Parse(json);
                }
                else
                {
                    foreach(var pair in secrets)
                    {
                        secrets[pair.Key] = pair.Value;
                    }
                }
            }
        }

        internal CodeWriter GenerateClass(SecretsConfig secretsConfig, IDictionary<string, string> secrets)
        {
            var writer = new CodeWriter();
            writer.AppendLine("// ------------------------------------------------------------------------------");
            writer.AppendLine("//  <autogenerated>");
            writer.AppendLine("//      This code was generated by Mobile.BuildTools. For more information  please visit");
            writer.AppendLine("//      https://mobilebuildtools.com or to file an issue please see");
            writer.AppendLine("//      https://github.com/dansiegel/Mobile.BuildTools");
            writer.AppendLine("//");
            writer.AppendLine("//      Changes to this file may cause incorrect behavior and will be lost when");
            writer.AppendLine("//      the code is regenerated.");
            writer.AppendLine("//");
            writer.AppendLine("//      When I wrote this, only God and I understood what I was doing");
            writer.AppendLine("//      Now, God only knows.");
            writer.AppendLine("//");
            writer.AppendLine("//      NOTE: This file should be excluded from source control.");
            writer.AppendLine("//  </autogenerated>");
            writer.AppendLine("// ------------------------------------------------------------------------------");
            writer.AppendLine();
            writer.AppendLine("using System;");
            writer.AppendLine("using GeneratedCodeAttribute = System.CodeDom.Compiler.GeneratedCodeAttribute;");
            using (writer.Block($"namespace {GetNamespace(secretsConfig.Namespace)}"))
            {
                writer.AppendAttribute(CompileGeneratedAttribute);
                using(writer.Block($"static partial class {secretsConfig.ClassName}"))
                {
                    foreach (var secret in secrets)
                    {
                        ProcessSecret(secret, secretsConfig, ref writer);
                        writer.AppendLine();
                    }
                }
            }

            return writer;
        }

        private void ProcessSecret(KeyValuePair<string, string> secret, SecretsConfig secretsConfig, ref CodeWriter writer)
        {
            if (!secretsConfig.HasKey(secret.Key, out var valueConfig))
            {
                return;
            }

            if (valueConfig is null)
            {
                valueConfig = GenerateValueConfig(secret, secretsConfig);
                if (secretsConfig.Properties is null)
                {
                    // Sanity Check
                    secretsConfig.Properties = new List<ValueConfig>();
                }
                secretsConfig.Properties.Add(valueConfig);
            }

            var mapping = valueConfig.PropertyType.GetPropertyTypeMapping();

            var standardOutput = PropertyBuilder(secret, mapping.Type, mapping.Handler, valueConfig.IsArray, false);
            var safeOutput = PropertyBuilder(secret, mapping.Type, mapping.Handler, valueConfig.IsArray, true);
            writer.AppendAttribute(CompileGeneratedAttribute);
            writer.AppendLine(standardOutput, safeOutput);
        }

        internal string ProcessSecret(KeyValuePair<string, string> secret, SecretsConfig secretsConfig, bool saveOutput, bool safeOutput = false)
        {
            if (!secretsConfig.HasKey(secret.Key, out var valueConfig) && !saveOutput)
            {
                return null;
            }

            if(valueConfig is null)
            {
                valueConfig = GenerateValueConfig(secret, secretsConfig);
                if(secretsConfig.Properties is null)
                {
                    // Sanity Check
                    secretsConfig.Properties = new List<ValueConfig>();
                }
                secretsConfig.Properties.Add(valueConfig);
            }

            var mapping = valueConfig.PropertyType.GetPropertyTypeMapping();
            return PropertyBuilder(secret, mapping.Type, mapping.Handler, valueConfig.IsArray, safeOutput);
        }

        internal ValueConfig GenerateValueConfig(KeyValuePair<string, string> secret, SecretsConfig config)
        {
            var value = secret.Value.ToString();
            var valueArray = Regex.Split(value, $"(?<!\\\\){config.Delimiter}").Select(x => x.Replace($"\\{config.Delimiter}", config.Delimiter));
            var isArray = false;
            if (valueArray.Count() > 1)
            {
                value = valueArray.FirstOrDefault();
            }
            var type = PropertyType.String;

            if(bool.TryParse(value, out _))
            {
                type = PropertyType.Bool;
            }
            else if (Regex.IsMatch(value, @"\d+\.\d+") && double.TryParse(value, out _))
            {
                type = PropertyType.Double;
            }
            else if (int.TryParse(value, out _))
            {
                type = PropertyType.Int;
            }

            return new ValueConfig
            {
                Name = secret.Key,
                IsArray = isArray,
                PropertyType = type
            };
        }

        internal string GetNamespace(string relativeNamespace)
        {
            var parts = relativeNamespace.Split('.', '/', '\\').Where(x => !string.IsNullOrEmpty(x));
            var count = parts.Count();
            if (count == 0)
            {
                return BaseNamespace;
            }
            else if(count == 1)
            {
                relativeNamespace = parts.First();
            }
            else if (count > 1)
            {
                relativeNamespace = string.Join(".", parts);
            }

            return $"{BaseNamespace}.{relativeNamespace}";
        }

        internal string PropertyBuilder(KeyValuePair<string, string> secret, Type type, IValueHandler valueHandler, bool isArray, bool safeOutput)
        {
            var output = string.Empty;
            var typeDeclaration = type.GetStandardTypeName();
            var accessModifier = type.FullName == typeDeclaration || isArray ? "static readonly" : "const";
            if (isArray)
            {
                typeDeclaration += "[]";
                var valueArray = GetValueArray(secret.Value).Select(x => valueHandler.Format(x, safeOutput));
                output = "new " + typeDeclaration + " { " + string.Join(", ", valueArray) + " }";
            }
            else
            {
                output = valueHandler.Format(secret.Value, safeOutput);
            }

            if(type == typeof(bool))
            {
                output = output.ToLower();
            }

            return $"internal {accessModifier} {typeDeclaration} {secret.Key} = {output};";
        }

        internal static IEnumerable<string> GetValueArray(JToken token) =>
            Regex.Split(token.ToString(), "(?<!\\\\);").Select(x => x.Replace("\\;", ";"));
    }
}
