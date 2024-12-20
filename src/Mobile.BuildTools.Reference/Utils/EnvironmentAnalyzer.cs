using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Handlers;
using Mobile.BuildTools.Models;

namespace Mobile.BuildTools.Utils
{
    public static class EnvironmentAnalyzer
    {
        private const string DefaultSecretPrefix = "BuildTools_";
        private const string LegacySecretPrefix = "Secret_";
        private const string DefaultManifestPrefix = "Manifest_";

        public static IDictionary<string, string> GatherEnvironmentVariables(IBuildConfiguration buildConfiguration = null, bool includeManifest = false)
        {
            var env = GetEnvironmentVariables(buildConfiguration);

            var configuration = buildConfiguration.BuildConfiguration;

            if (buildConfiguration is null)
            {
                foreach (var key in Environment.GetEnvironmentVariables().Keys)
                {
                    env[key.ToString()] = Environment.GetEnvironmentVariable(key.ToString());
                }

                return env;
            }

            var projectDirectory = new DirectoryInfo(buildConfiguration.ProjectDirectory);
            var solutionDirectory = new DirectoryInfo(buildConfiguration.SolutionDirectory);
            var directories = new List<DirectoryInfo>
            {
                solutionDirectory,
                projectDirectory
            };

            if (buildConfiguration.Platform != Platform.Unsupported)
            {
                new DirectoryInfo[]
                {
                    new (Path.Combine(projectDirectory.FullName, buildConfiguration.Platform.ToString())),
                    new (Path.Combine(projectDirectory.FullName, "Platforms", buildConfiguration.Platform.ToString()))
                }
                .Where(x => x.Exists)
                .ForEach(directories.Add);
            }

            var stoppingDir = solutionDirectory.Parent;
            var lookupDir = projectDirectory;
            do
            {
                lookupDir = lookupDir.Parent;
                if (lookupDir is null || stoppingDir.FullName == lookupDir.FullName)
                {
                    break;
                }
                else if (!directories.Contains(lookupDir))
                {
                    directories.Add(lookupDir);
                }
            } while (lookupDir != solutionDirectory);

            directories = directories.Select(x =>
            {
                var dir = x.FullName;
                if (dir.EndsWith($"{Path.DirectorySeparatorChar}"))
                    dir = dir.Substring(0, dir.Length - 1);

                return dir;
            })
                .Distinct()
                .Select(x => new DirectoryInfo(x))
                .Where(x => x.Exists)
                .ToList();

            string[] expectedFileNames =
            [
                Constants.SecretsJsonFileName,
                string.Format(Constants.SecretsJsonConfigurationFileFormat, configuration)
            ];
            foreach (var fileName in expectedFileNames)
            {
                foreach (var directory in directories)
                {
                    var file = new FileInfo(Path.Combine(directory.FullName, fileName));
                    if (file.Exists)
                    {
                        buildConfiguration.Logger.LogWarning($"The secrets.json has been deprecated and will no longer be supported in a future version. Please migrate '{fileName}' to appsettings.json");
                        LoadSecrets(file.FullName, ref env);
                        break;
                    }
                }
            }

            foreach (var directory in directories)
            {
                var files = directory.EnumerateFiles("*.json", SearchOption.TopDirectoryOnly);
                if (!files.Any(x => x.Name.StartsWith("appsettings")))
                    continue;

                var didLoad = false;
                bool TryLoadFile(string fileName)
                {
                    if (files.Any(x => x.Name == fileName))
                    {
                        didLoad = true;
                        LoadSecrets(Path.Combine(directory.FullName, fileName), ref env);
                        return true;
                    }

                    return false;
                }

                var searchConfiguration = configuration;
                if (buildConfiguration.Configuration.Environment?.EnableFuzzyMatching ?? false)
                {
                    var availableMatches = files.Select(x =>
                    {
                        var match = Regex.Match(x.Name, @"appsettings\.(?<config>[a-zA-Z0-9]+)\.json");
                        if (match.Success)
                        {
                            return match.Groups["config"].Value;
                        }
                        return null;
                    }).Where(x => !string.IsNullOrEmpty(x));

                    if (!availableMatches.Any(x => x == searchConfiguration) && availableMatches.Any(searchConfiguration.Contains))
                    {
                        searchConfiguration = availableMatches.First(searchConfiguration.Contains);
                    }
                }

                TryLoadFile(Constants.AppSettingsJsonFileName);
                TryLoadFile(string.Format(Constants.AppSettingsJsonConfigurationFileFormat, searchConfiguration));
                TryLoadFile(string.Format(Constants.AppSettingsJsonConfigurationFileFormat, $"{buildConfiguration.Platform}"));
                TryLoadFile(string.Format(Constants.AppSettingsJsonConfigurationFileFormat, $"{buildConfiguration.Platform}.{searchConfiguration}"));

                if (didLoad)
                    break;
            }

            if (includeManifest)
            {
                directories
                    .SelectMany(x =>
                        x.EnumerateFiles("*.json", SearchOption.TopDirectoryOnly)
                            .Where(x => x.Name == Constants.ManifestJsonFileName))
                    .ForEach(x => LoadSecrets(x.FullName, ref env));
                LoadSecrets(Path.Combine(projectDirectory.FullName, Constants.ManifestJsonFileName), ref env);
                LoadSecrets(Path.Combine(solutionDirectory.FullName, Constants.ManifestJsonFileName), ref env);
            }

            if (buildConfiguration.Configuration.Environment?.EnableFuzzyMatching ?? false)
            {
                var keys = env.Keys.ToArray();
                foreach (var key in keys)
                {
                    var match = Regex.Match(key, @"^(?<prefix>[a-zA-Z0-9]+)_(?<key>.+)$");
                    if (match.Success)
                    {
                        var newKey = match.Groups["key"].Value;
                        if (!env.ContainsKey(newKey))
                        {
                            env[newKey] = env[key];
                        }
                    }
                }
            }

            return new SortedDictionary<string, string>(env);
        }

        private static Dictionary<string, string> GetEnvironmentVariables(IBuildConfiguration buildConfiguration)
        {
            var env = new Dictionary<string, string>();
            var configuration = buildConfiguration.BuildConfiguration;
            if (buildConfiguration?.Configuration?.Environment != null)
            {
                var settings = buildConfiguration.Configuration.Environment;
                var defaultSettings = settings.Defaults ?? [];
                if (settings.Configuration is not null)
                {
                    bool ContainsConfigKey(string key, [MaybeNullWhen(false)] out string configurationKey)
                    {
                        if (settings.Configuration.ContainsKey(key))
                        {
                            configurationKey = key;
                            return true;
                        }
                        else if (settings.EnableFuzzyMatching)
                        {
                            var availableKey = settings.Configuration.Keys.FirstOrDefault(key.StartsWith);
                            if (!string.IsNullOrEmpty(availableKey))
                            {
                                configurationKey = availableKey;
                                return true;
                            }
                        }

                        configurationKey = null;
                        return false;
                    }

                    void MergeDefaultSettings(string configurationKey)
                    {
                        foreach ((var key, var value) in settings.Configuration[configurationKey])
                            defaultSettings[key] = value;
                    }

                    if (ContainsConfigKey(configuration, out var configurationKey))
                        MergeDefaultSettings(configurationKey);
                    if (ContainsConfigKey($"{buildConfiguration.Platform}", out configurationKey))
                        MergeDefaultSettings(configurationKey);
                    if (ContainsConfigKey($"{buildConfiguration.Platform}_{configuration}", out configurationKey))
                        MergeDefaultSettings(configurationKey);
                }

                UpdateVariables(defaultSettings, ref env);
            }

            return env;
        }

        internal static void UpdateVariables(IDictionary<string, string> settings, ref Dictionary<string, string> output)
        {
            if (settings is null || settings.Count < 1)
                return;

            foreach((var key, var value) in settings)
            {
                if (!output.ContainsKey(key))
                    output[key] = value;
            }
        }

        // This should stop looking when:
        // - We have found a solution
        // - Either the Current directory or Parent is the Root i.e. c:\
        // - Either the Current directory or Parent is the User directory i.e. c:\Users\Dan
        // - The Current directory contains the .git folder
        public static string LocateSolution(string searchDirectory)
        {
            var di = new DirectoryInfo(searchDirectory);
            if (di.EnumerateFiles("*.sln").Any() 
                || IsRootPath(di.Parent) 
                || di.EnumerateDirectories().Any(x => x.Name == ".git")
                || IsRootPath(di))
            {
                return searchDirectory;
            }

            return LocateSolution(Directory.GetParent(searchDirectory).FullName);
        }

        public static IEnumerable<string> GetManifestPrefixes(Platform platform, string knownPrefix)
        {
            var prefixes = new List<string>(GetSecretPrefixes(platform, forceIncludeDefault: true))
            {
                DefaultManifestPrefix
            };

            if(!string.IsNullOrEmpty(knownPrefix))
            {
                prefixes.Add(knownPrefix);
            }

            var platformPrefix = GetPlatformManifestPrefix(platform);
            if(!string.IsNullOrWhiteSpace(platformPrefix))
            {
                prefixes.Add(platformPrefix);
            }

            return prefixes;
        }

        private static string GetPlatformManifestPrefix(Platform platform) => 
            platform switch
            {
                Platform.Android => "DroidManifest_",
                Platform.iOS => "iOSManifest_",
                Platform.Windows => "WindowsManifest_",
                Platform.UWP => "UWPManifest_",
                Platform.macOS => "MacManifest_",
                Platform.Tizen => "TizenManifest_",
                _ => null,
            };

        public static string[] GetPlatformSecretPrefix(Platform platform) =>
            platform switch
            {
                Platform.Android => ["DroidSecret_"],
                Platform.iOS => ["iOSSecret_"],
                Platform.Windows => ["WindowsSecret_"],
                Platform.UWP => ["UWPSecret_"],
                Platform.macOS => ["MacSecret_"],
                Platform.Tizen => ["TizenSecret_"],
                _ => [DefaultSecretPrefix, LegacySecretPrefix],
            };

        public static IEnumerable<string> GetSecretPrefixes(Platform platform, bool forceIncludeDefault = false)
        {
            var prefixes = new List<string>(GetPlatformSecretPrefix(platform))
            {
                "SharedSecret_"
            };

            if(platform != Platform.Unsupported)
            {
                prefixes.Add("PlatformSecret_");
            }

            if(forceIncludeDefault && !prefixes.Contains(DefaultSecretPrefix))
            {
                prefixes.Add(DefaultSecretPrefix);
                prefixes.Add($"MB{DefaultManifestPrefix}");
            }

            return prefixes;
        }

        public static IEnumerable<string> GetSecretKeys(IEnumerable<string> prefixes)
        {
            var variables = GatherEnvironmentVariables();
            return variables.Keys.Where(k => prefixes.Any(p => k.StartsWith(p)));
        }

        public static IDictionary<string, string> GetSecrets(IBuildConfiguration build, string knownPrefix)
        {
            var prefixes = GetSecretPrefixes(build.Platform);
            if(!string.IsNullOrEmpty(knownPrefix))
            {
                prefixes = new List<string>(prefixes)
                {
                    knownPrefix
                };
            }
            var keys = GetSecretKeys(prefixes);
            var variables = GatherEnvironmentVariables().Where(p => keys.Any(k => k == p.Key));
            var output = new Dictionary<string, string>();
            foreach(var prefix in prefixes)
            {
                foreach(var pair in variables.Where(v => v.Key.StartsWith(prefix)))
                {
                    var key = Regex.Replace(pair.Key, prefix, "");
                    output.Add(key, pair.Value);
                }
            }

            if (build?.Configuration?.Environment != null)
            {
                var configuration = build.BuildConfiguration;
                var settings = build.Configuration.Environment;
                var defaultSettings = settings.Defaults ?? [];
                if (settings.Configuration != null && settings.Configuration.ContainsKey(configuration))
                {
                    foreach ((var key, var value) in settings.Configuration[configuration])
                        defaultSettings[key] = value;
                }

                UpdateVariables(defaultSettings, ref output);
            }

            return output;
        }

        public static bool IsInGitRepo(string projectPath)
        {
            var di = new DirectoryInfo(projectPath);
            if (di.EnumerateDirectories().Any(x => x.Name == ".git"))
                return true;

            if (IsRootPath(di))
                return false;

            return IsInGitRepo(di.Parent.FullName);
        }

        private static bool IsRootPath(DirectoryInfo directoryPath) =>
            directoryPath.Root.FullName == directoryPath.FullName ||
                directoryPath.FullName == Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        private static void LoadSecrets(string path, ref Dictionary<string, string> env)
        {
            if (!File.Exists(path)) return;

            var json = File.ReadAllText(path);
            var document = JsonDocument.Parse(json);
            foreach(var setting in document.RootElement.EnumerateObject())
            {
                env[setting.Name] = setting.GetPropertyValueAsString();
            }
        }
    }
}
