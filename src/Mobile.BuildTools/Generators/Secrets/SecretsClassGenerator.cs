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
using Mobile.BuildTools.Models.Settings;
using Mobile.BuildTools.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mobile.BuildTools.Generators.Secrets
{
    internal class SecretsClassGenerator : GeneratorBase<List<ITaskItem>>
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
            Outputs = new List<ITaskItem>();
        }
#pragma warning restore IDE1006, IDE0040

        public string RootNamespace { get; set; }

        protected override void ExecuteInternal()
        {
            var settingsConfig = Build.GetSettingsConfig();
            foreach(var settings in settingsConfig)
            {
                ProcessSettingsConfig(settings);
            }
        }

        private void ProcessSettingsConfig(SettingsConfig settingsConfig)
        {
            if (!SecretsFileLocations.Any(x => File.Exists(x)))
            {
                Log.LogMessage("No secrets.json was found in either the Project or Solution Directory.");

                if (settingsConfig.Properties.Any())
                {
                    var jObject = new JObject();
                    foreach (var propConfig in settingsConfig.Properties)
                    {
                        jObject[propConfig.Name] = $"{propConfig.PropertyType}";
                        Log.LogError($"Missing Secret parameter: {propConfig.Name}");
                    }

                    Log.LogWarning(@$"To fix the build please add a secrets.json in either the Project or Solution root directory.");
                    Log.LogMessage(jObject.ToString(Formatting.Indented));
                }

                return;
            }

            var secrets = GetMergedSecrets();

            var replacement = string.Empty;
            var safeReplacement = string.Empty;

            if (string.IsNullOrEmpty(settingsConfig.ClassName))
                settingsConfig.ClassName = "Secrets";

            if (string.IsNullOrEmpty(settingsConfig.Namespace))
                settingsConfig.Namespace = "Helpers";

            if (string.IsNullOrEmpty(settingsConfig.Delimiter))
                settingsConfig.Delimiter = ";";

            if (string.IsNullOrEmpty(settingsConfig.Prefix))
                settingsConfig.Prefix = "BuildTools_";

            foreach (var propConfig in settingsConfig.Properties)
            {
                if (string.IsNullOrEmpty(propConfig.DefaultValue))
                    continue;
                if (secrets.ContainsKey(propConfig.Name) || secrets.ContainsKey($"{settingsConfig.Prefix}{propConfig.Name}"))
                    continue;

                secrets.Add(propConfig.Name, propConfig.DefaultValue == "null" || propConfig.DefaultValue == "default" ? null : propConfig.DefaultValue);
            }

            var hasErrors = false;
            foreach(var propConfig in settingsConfig.Properties)
            {
                if (!secrets.ContainsKey(propConfig.Name) && !secrets.ContainsKey($"{settingsConfig.Prefix}{propConfig.Name}"))
                {
                    hasErrors = true;
                    Log.LogError($"Missing Secret parameter: {propConfig.Name}");
                }
            }

            if (hasErrors)
                return;

            var writer = GenerateClass(settingsConfig, secrets);
            var secretsClass = writer.ToString();
            Log.LogMessage(Build.Configuration.Debug ? secretsClass : writer.SafeOutput);

            var namespacePath = string.Join($"{Path.PathSeparator}", settingsConfig.Namespace.Split('.').Where(x => !string.IsNullOrEmpty(x)));
            var fileName = $"{settingsConfig.ClassName}.g.cs";
            var projectFile = Path.Combine(namespacePath, fileName);
            var intermediateFile = Path.Combine(Build.IntermediateOutputPath, namespacePath, fileName);

            Log.LogMessage($"Writing Secrets Class to: '{intermediateFile}'");
            var generatedFile = new TaskItem(intermediateFile);
            generatedFile.SetMetadata("Visible", bool.TrueString);
            generatedFile.SetMetadata("Link", projectFile);
            if(File.Exists(projectFile))
            {
                generatedFile.SetMetadata("DependentUpon", $"{settingsConfig.ClassName}.cs");
            }

            Outputs.Add(generatedFile);

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

        internal CodeWriter GenerateClass(SettingsConfig settingsConfig, IDictionary<string, string> secrets)
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
            using (writer.Block($"namespace {GetNamespace(settingsConfig)}"))
            {
                writer.AppendAttribute(CompileGeneratedAttribute);
                using(writer.Block($"{settingsConfig.Accessibility.ToString().ToLower()} static partial class {settingsConfig.ClassName}"))
                {
                    foreach (var secret in secrets)
                    {
                        try
                        {
                            ProcessSecret(secret, settingsConfig, ref writer);
                            writer.AppendLine();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"An error occurred while processing {secret.Key}.", ex);
                        }
                    }
                }
            }

            return writer;
        }

        private void ProcessSecret(KeyValuePair<string, string> secret, SettingsConfig secretsConfig, ref CodeWriter writer)
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
            var accessibility = secretsConfig.Accessibility.ToString().ToLower();

            var standardOutput = PropertyBuilder(secret, mapping.Type, mapping.Handler, valueConfig.IsArray, false, accessibility);
            var safeOutput = PropertyBuilder(secret, mapping.Type, mapping.Handler, valueConfig.IsArray, true, accessibility);
            writer.AppendAttribute(CompileGeneratedAttribute);
            writer.AppendLine(standardOutput, safeOutput);
        }

        internal string ProcessSecret(KeyValuePair<string, string> secret, SettingsConfig secretsConfig, bool saveOutput, bool safeOutput = false)
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
            var accessibility = secretsConfig.Accessibility.ToString().ToLower();

            return PropertyBuilder(secret, mapping.Type, mapping.Handler, valueConfig.IsArray, safeOutput, accessibility);
        }

        internal ValueConfig GenerateValueConfig(KeyValuePair<string, string> secret, SettingsConfig config)
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

        internal string GetNamespace(SettingsConfig settings)
        {
            var relativeNamespace = settings.Namespace;
            var rootNamespace = string.IsNullOrEmpty(settings.RootNamespace) ? RootNamespace : settings.RootNamespace;

            if (string.IsNullOrEmpty(relativeNamespace))
                relativeNamespace = "Helpers";

            var parts = relativeNamespace.Split('.', '/', '\\').Where(x => !string.IsNullOrEmpty(x));
            var count = parts.Count();
            if (count == 0)
            {
                return rootNamespace;
            }
            else if(count == 1)
            {
                relativeNamespace = parts.First();
            }
            else if (count > 1)
            {
                relativeNamespace = string.Join(".", parts);
            }

            return $"{rootNamespace}.{relativeNamespace}";
        }

        internal string PropertyBuilder(KeyValuePair<string, string> secret, Type type, IValueHandler valueHandler, bool isArray, bool safeOutput, string accessibility)
        {
            var output = string.Empty;
            var typeDeclaration = type.GetStandardTypeName();
            var accessModifier = type.FullName == typeDeclaration || isArray ? "static readonly" : "const";
            if(secret.Value is null || secret.Value == "null" || secret.Value == "default")
            {
                if(type == typeof(bool) && !isArray)
                {
                    output = bool.FalseString.ToLower();
                }
                else if(isArray)
                {
                    output = $"System.Array.Empty<{typeDeclaration}>()";
                    typeDeclaration += "[]";
                }
                else
                {
                    output = "default";
                }
            }
            else if (isArray)
            {
                typeDeclaration += "[]";
                var valueArray = GetValueArray(secret.Value).Select(x => valueHandler.Format(x, safeOutput));
                output = "new " + typeDeclaration + " { " + string.Join(", ", valueArray) + " }";
                if (type == typeof(bool))
                {
                    output = output.ToLower();
                }
            }
            else
            {
                output = valueHandler.Format(secret.Value, safeOutput);
                if (type == typeof(bool))
                {
                    output = output.ToLower();
                }
            }

            return $"{accessibility} {accessModifier} {typeDeclaration} {secret.Key} = {output};";
        }

        internal static IEnumerable<string> GetValueArray(JToken token) =>
            Regex.Split(token.ToString(), "(?<!\\\\);").Select(x => x.Replace("\\;", ";"));
    }
}
