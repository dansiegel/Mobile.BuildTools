using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Handlers;
using Mobile.BuildTools.Models.Settings;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Generators.Secrets
{
    internal class SecretsClassGenerator : GeneratorBase<List<ITaskItem>>
    {
#pragma warning disable IDE1006, IDE0040
        private string CompileGeneratedAttribute { get; }

        public SecretsClassGenerator(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
            var assembly = typeof(SecretsClassGenerator).Assembly;
            var toolVersion = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
            CompileGeneratedAttribute = @$"[GeneratedCodeAttribute(""Mobile.BuildTools.Generators.Secrets.SecretsClassGenerator"", ""{toolVersion}"")]";

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
            var replacement = string.Empty;
            var safeReplacement = string.Empty;

            if (string.IsNullOrEmpty(settingsConfig.ClassName))
                settingsConfig.ClassName = "AppSettings";

            if (string.IsNullOrEmpty(settingsConfig.Namespace))
                settingsConfig.Namespace = "Helpers";

            if (string.IsNullOrEmpty(settingsConfig.Delimiter))
                settingsConfig.Delimiter = ";";

            if (string.IsNullOrEmpty(settingsConfig.Prefix))
                settingsConfig.Prefix = "BuildTools_";

            var secrets = GetMergedSecrets(settingsConfig, out var hasErrors);

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

        internal IDictionary<string, string> GetMergedSecrets(SettingsConfig settingsConfig, out bool hasErrors)
        {
            if (string.IsNullOrEmpty(settingsConfig.Prefix))
                settingsConfig.Prefix = "BuildTools_";

            var env = EnvironmentAnalyzer.GatherEnvironmentVariables(Build);
            var secrets = new Dictionary<string, string>();
            hasErrors = false;
            foreach(var prop in settingsConfig.Properties)
            {
                var searchKeys = new[]
                {
                    $"{settingsConfig.Prefix}{prop.Name}",
                    $"{settingsConfig.Prefix}_{prop.Name}",
                    prop.Name,
                };

                string key = null;
                foreach(var searchKey in searchKeys)
                {
                    if (!string.IsNullOrEmpty(key))
                        break;

                    key = env.Keys.FirstOrDefault(x =>
                        x.Equals(searchKey, StringComparison.InvariantCultureIgnoreCase));
                }

                if(string.IsNullOrEmpty(key))
                {
                    if(string.IsNullOrEmpty(prop.DefaultValue))
                    {
                        Log.LogError($"Missing Settings Key: {prop.Name}");
                        hasErrors = true;
                        continue;
                    }

                    secrets[prop.Name] = prop.DefaultValue == "null" || prop.DefaultValue == "default" ? null : prop.DefaultValue;
                    continue;
                }

                secrets[prop.Name] = env[key];
            }

            return secrets;
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

        internal string PropertyBuilder(KeyValuePair<string, string> secret, Type type, IValueHandler valueHandler, bool? isArrayNullable, bool safeOutput, string accessibility)
        {
            var isArray = isArrayNullable ?? false;
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
                var valueArray = GetValueArray(secret.Value.ToString()).Select(x => valueHandler.Format(x, safeOutput));
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

        internal static IEnumerable<string> GetValueArray(JsonNode token) =>
            Regex.Split(token.ToString(), "(?<!\\\\);").Select(x => x.Replace("\\;", ";"));
    }
}
