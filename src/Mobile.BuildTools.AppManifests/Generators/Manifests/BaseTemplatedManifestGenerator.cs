using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Mobile.BuildTools.Build;

namespace Mobile.BuildTools.Generators.Manifests;

internal abstract class BaseTemplatedManifestGenerator : GeneratorBase<string>
{
    internal const string DefaultToken = "$$";

    protected BaseTemplatedManifestGenerator(IBuildConfiguration configuration) : base(configuration)
    {
        Token = string.IsNullOrEmpty(configuration.Configuration.Manifests.Token)
            ? DefaultToken : configuration.Configuration.Manifests.Token;
    }

    public string Token { get; }
    public string ProjectDirectory => Build.ProjectDirectory;
    public bool ApplyPackageName { get; set; } = true;
    public bool EnableTokenReplacement { get; set; } = true;
    public string ManifestInputPath { get; set; }
    public string ManifestOutputPath { get; set; }

    protected override void ExecuteInternal()
    {
        if (!File.Exists(ManifestInputPath))
        {
            Log?.LogWarning("There is no template manifest at '{0}'.", ManifestInputPath);
            return;
        }

        var variables = Utils.EnvironmentAnalyzer.GatherEnvironmentVariables(Build, true);
        var manifest = EnableTokenReplacement ? TransformManifest(ReadManifest(), variables) : ReadManifest();
        if (EnableTokenReplacement && ApplyPackageName && variables.TryGetValue(Constants.AppPackageName, out var packageName))
            manifest = SetAppBundleId(manifest, packageName);

        SaveManifest(manifest);
        Outputs = ManifestOutputPath;
    }

    public abstract string GetBundId();
    protected abstract string SetAppBundleId(string manifest, string packageName);
    protected virtual string ReadManifest() => File.ReadAllText(ManifestInputPath);
    protected virtual void SaveManifest(string manifest) => WriteManifest(manifest);

    protected virtual string TransformManifest(string manifest, IDictionary<string, string> variables) =>
        ReplaceTokens(manifest, variables);

    internal MatchCollection GetMatches(string template) =>
        Regex.Matches(template, $"{Regex.Escape(Token)}(.+?){Regex.Escape(Token)}");

    protected string ReplaceTokens(string text, IDictionary<string, string> variables)
    {
        // Evaluate only original matches, so a replacement containing another token stays literal.
        return Regex.Replace(text, $"{Regex.Escape(Token)}(.+?){Regex.Escape(Token)}", match =>
        {
            if (IsFrameworkToken(match.Groups[1].Value))
                return match.Value;
            var key = GetKey(match.Groups[1].Value, variables);
            if (key != null && variables[key] != null)
            {
                Log?.LogMessage($"Replacing manifest token '{match.Groups[1].Value}'.");
                return variables[key];
            }

            var message = $"Unable to locate replacement value for '{match.Value}'.";
            if (Build.Configuration.Manifests.MissingTokensAsErrors)
                Log?.LogError(message);
            else
                Log?.LogWarning(message);
            return match.Value;
        });
    }

    protected virtual bool IsFrameworkToken(string name) => false;

    internal string ProcessMatch(string template, Match match, IDictionary<string, string> variables)
    {
        var key = GetKey(match.Groups[1].Value, variables);
        if (key != null && variables[key] != null)
            return template.Replace(match.Value, variables[key]);
        ReplaceTokens(match.Value, variables);
        return template;
    }

    internal string GetKey(string name, IDictionary<string, string> variables)
    {
        if (variables.ContainsKey(name))
            return name;
        return Utils.EnvironmentAnalyzer.GetManifestPrefixes(Build.Platform, Build.Configuration.Manifests.VariablePrefix)
            .Select(prefix => prefix + name).FirstOrDefault(variables.ContainsKey);
    }

    internal void WriteManifest(string manifest)
    {
        if (File.Exists(ManifestOutputPath) && File.ReadAllText(ManifestOutputPath) == manifest)
            return;
        var directory = Path.GetDirectoryName(ManifestOutputPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
        File.WriteAllText(ManifestOutputPath, manifest);
    }
}
