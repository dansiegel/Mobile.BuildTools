using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Logging;
using Newtonsoft.Json.Linq;

namespace Mobile.BuildTools.Generators
{
    public class SecretsClassGenerator : IGenerator
    {
#pragma warning disable IDE1006, IDE0040
        private const string AutoGeneratedMessage =
@"// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by Mobile.BuildTools. For more information or to
//      file an issue please see https://github.com/dansiegel/Mobile.BuildTools
//
//      Changes to this file may cause incorrect behavior and will be lost when 
//      the code is regenerated. 
//
//      NOTE: This file should be excluded from source control.
//  </autogenerated>
// ------------------------------------------------------------------------------

";

        private const string SafePlaceholder = "*****";

        private const string TabSpace = "    ";
#pragma warning restore IDE1006, IDE0040

        public string ProjectBasePath { get; set; }

        public string SecretsClassName { get; set; }

        public string ConfigurationSecretsJsonFilePath { get; set; }

        public string SecretsJsonFilePath { get; set; }

        public string BaseNamespace { get; set; }

        public string OutputPath { get; set; }

        public string IntermediateOutputPath { get; set; }

        public bool? DebugOutput { get; set; }

        public ILog Log { get; set; }

        public ITaskItem[] GeneratedFiles { get; private set; }

        public void Execute()
        {
            if (DebugOutput == null)
            {
                DebugOutput = false;
            }

            var secrets = GetMergedSecrets();

            var replacement = string.Empty;
            var safeReplacement = string.Empty;

            foreach (var secret in secrets)
            {
                replacement += ProcessSecret(secret);
                safeReplacement += ProcessSecret(secret, true);
            }

            replacement = Regex.Replace(replacement, "\n\n$", "");

            var secretsClass = GenerateClass(replacement);
            Log.LogMessage((bool)DebugOutput ? secretsClass : GenerateClass(Regex.Replace(safeReplacement, "\n\n$", "")));

            var projectFile = Path.Combine(OutputPath, $"{SecretsClassName}.cs");
            var intermediateFile = Path.Combine(IntermediateOutputPath, $"{SecretsClassName}.cs");
            var outputFile = File.Exists(projectFile) ? projectFile : intermediateFile;
            Log.LogMessage($"Writing Secrets Class to: '{outputFile}'");
            GeneratedFiles = new ITaskItem[] {
                new TaskItem(ProjectCollection.Escape(outputFile))
            };

            File.WriteAllText(outputFile, secretsClass);
        }

        internal JObject GetMergedSecrets()
        {
            JObject secrets = null;
            CreateOrMerge(SecretsJsonFilePath, ref secrets);
            CreateOrMerge(ConfigurationSecretsJsonFilePath, ref secrets);

            if(secrets is null)
            {
                throw new Exception("An unexpected error occurred. Could not locate any secrets.");
            }

            return secrets;
        }

        internal void CreateOrMerge(string jsonFilePath, ref JObject secrets)
        {
            if(File.Exists(jsonFilePath))
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

        internal string GenerateClass(string replacement) =>
            $"{AutoGeneratedMessage}namespace {GetNamespace()}\n{{\n{TabSpace}internal static class {SecretsClassName}\n{TabSpace}{{\n{replacement}\n{TabSpace}}}\n}}\n";

        internal string ProcessSecret(KeyValuePair<string, JToken> secret, bool safeOutput = false)
        {
            var value = secret.Value.ToString();
            var valueArray = Regex.Split(value, "(?<!\\\\);").Select(x => x.Replace("\\;", ";"));

            if(valueArray.Count() > 1)
            {
                return GetArrayProperty(secret, valueArray, safeOutput);
            }

            return GetStandardProperty(secret, value, safeOutput);
        }

        internal string GetStandardProperty(KeyValuePair<string, JToken> secret, string value, bool safeOutput)
        {
            var outputValue = safeOutput ? SafePlaceholder : value;
            if (bool.TryParse(value, out _))
            {
                return $"{TabSpace}{TabSpace}internal const bool {secret.Key} = {outputValue.ToLower()};\n\n";
            }
            else if (Regex.IsMatch(value, @"\d+\.\d+") && double.TryParse(value, out _))
            {
                return $"{TabSpace}{TabSpace}internal const double {secret.Key} = {outputValue};\n\n";
            }
            else if (int.TryParse(value, out _))
            {
                return $"{TabSpace}{TabSpace}internal const int {secret.Key} = {outputValue};\n\n";
            }
            else
            {
                return $"{TabSpace}{TabSpace}internal const string {secret.Key} = \"{outputValue}\";\n\n";
            }
        }

        internal string GetArrayProperty(KeyValuePair<string, JToken> secret, IEnumerable<string> values, bool safeOutput)
        {
            var checkValue = values.First();
            //var outputValue = safeOutput ? SafePlaceholder : value;
            if (bool.TryParse(checkValue, out _))
            {
                var outputValue = safeOutput ? SafePlaceholder : string.Join(", ", values);
                return $"{TabSpace}{TabSpace}internal const bool[] {secret.Key} = new bool[] {{ {outputValue} }};\n\n";
            }
            else if (Regex.IsMatch(checkValue, @"\d+\.\d+") && double.TryParse(checkValue, out _))
            {
                var outputValue = safeOutput ? SafePlaceholder : string.Join(", ", values);
                return $"{TabSpace}{TabSpace}internal const double[] {secret.Key} = new double[] {{ {outputValue} }};\n\n";
            }
            else if (int.TryParse(checkValue, out _))
            {
                var outputValue = safeOutput ? SafePlaceholder : string.Join(", ", values);
                return $"{TabSpace}{TabSpace}internal const int[] {secret.Key} = new int[] {{ {outputValue} }};\n\n";
            }
            else
            {
                var outputValue = safeOutput ? SafePlaceholder : string.Join(", ", values.Select(x => $"\"{x}\""));
                return $"{TabSpace}{TabSpace}internal const string[] {secret.Key} = new string[] {{ {outputValue} }};\n\n";
            }
        }

        internal string GetNamespace()
        {
            var file = new Uri(OutputPath);
            // Must end in a slash to indicate folder
            var folder = new Uri(ProjectBasePath);
            var relativePath =
            Uri.UnescapeDataString(
                folder.MakeRelativeUri(file)
                      .ToString()
                      .Replace('/', '.')
                );

            if(!string.IsNullOrWhiteSpace(BaseNamespace))
            {
                var folderName = Path.GetFileName(ProjectBasePath);
                relativePath = Regex.Replace(relativePath, folderName, BaseNamespace);
            }
            else
            {
                Log.LogWarning($"Using Fallback namespace: {relativePath}");
            }

            return relativePath;
        }
    }
}