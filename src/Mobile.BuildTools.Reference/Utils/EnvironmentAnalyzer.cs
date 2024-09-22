using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Handlers;
using Mobile.BuildTools.Reference.IO;

namespace Mobile.BuildTools.Utils
{
    public static class EnvironmentAnalyzer
    {
        private const string DefaultSecretPrefix = "BuildTools_";
        private const string LegacySecretPrefix = "Secret_";
        private const string DefaultManifestPrefix = "Manifest_";

        public static IDictionary<string, string> GatherEnvironmentVariables(IBuildConfiguration buildConfiguration = null, bool includeManifest = false)
        {
            var env = new Dictionary<string, string>();
            if(buildConfiguration is null)
            {
                foreach (var key in Environment.GetEnvironmentVariables().Keys)
                {
                    env[key.ToString()] = Environment.GetEnvironmentVariable(key.ToString());
                }

                return env; 
            }

            env = GetEnvironmentVariables(buildConfiguration);

            var projectDirectory = new DirectoryInfo(buildConfiguration.ProjectDirectory);
            var solutionDirectory = new DirectoryInfo(buildConfiguration.SolutionDirectory);
            var configuration = buildConfiguration.BuildConfiguration;
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
                if (lookupDir is null || stoppingDir == lookupDir)
                {
                    break;
                }
                else if(!directories.Contains(lookupDir))
                {
                    directories.Add(lookupDir);
                }
            } while (lookupDir != solutionDirectory);

            directories
                .Distinct()
                .SelectMany(x =>
                    x.EnumerateFiles("*.json", SearchOption.TopDirectoryOnly)
                        .Where(x => x.Name == Constants.SecretsJsonFileName || x.Name == string.Format(Constants.SecretsJsonConfigurationFileFormat, configuration)))
                .ForEach(x =>
                {
                    if (x.Exists)
                    {
                        buildConfiguration.Logger.LogWarning($"The secrets.json has been deprecated and will no longer be supported in a future version. Please migrate '{x}' to appsettings.json");
                        LoadSecrets(x.FullName, ref env);
                    }
                });

            directories
                .Distinct(DirectoryInfoEqualityComparer.Instance)
                .SelectMany(x => 
                    x.EnumerateFiles("*.json", SearchOption.TopDirectoryOnly)
                        .Where(x => x.Name == Constants.AppSettingsJsonFileName 
                            || x.Name == string.Format(Constants.AppSettingsJsonConfigurationFileFormat, configuration)
                            || x.Name == string.Format(Constants.AppSettingsJsonConfigurationFileFormat, $"{buildConfiguration.Platform}")
                            || x.Name == string.Format(Constants.AppSettingsJsonConfigurationFileFormat, $"{buildConfiguration.Platform}.{configuration}")))
                .ForEach(x => LoadSecrets(x.FullName, ref env));

            if (includeManifest)
            {
                directories
                    .Distinct(DirectoryInfoEqualityComparer.Instance)
                    .SelectMany(x =>
                        x.EnumerateFiles("*.json", SearchOption.TopDirectoryOnly)
                            .Where(x => x.Name == Constants.ManifestJsonFileName))
                    .ForEach(x => LoadSecrets(x.FullName, ref env));
                LoadSecrets(Path.Combine(projectDirectory.FullName, Constants.ManifestJsonFileName), ref env);
                LoadSecrets(Path.Combine(solutionDirectory.FullName, Constants.ManifestJsonFileName), ref env);
            }

            if(buildConfiguration?.Configuration?.Environment != null)
            {
                var settings = buildConfiguration.Configuration.Environment;
                var defaultSettings = settings.Defaults ?? [];
                if(settings.Configuration != null && settings.Configuration.ContainsKey(configuration))
                {
                    foreach ((var key, var value) in settings.Configuration[configuration])
                        defaultSettings[key] = value;
                }

                UpdateVariables(defaultSettings, ref env);
            }

            return env;
        }

        private static Dictionary<string, string> GetEnvironmentVariables(IBuildConfiguration buildConfiguration)
        {
            var env = new Dictionary<string, string>();
            foreach((var key, var value) in buildConfiguration.Configuration.Environment.Defaults)
            {
                env[key] = value;
            }

            if(buildConfiguration.Configuration.Environment.Configuration.ContainsKey(buildConfiguration.BuildConfiguration))
            {
                var configEnvironment = buildConfiguration.Configuration.Environment.Configuration[buildConfiguration.BuildConfiguration];
                if(configEnvironment is not null)
                {
                    foreach((var key, var value) in configEnvironment)
                    {
                        env[key] = value;
                    }
                }
            }

            foreach (var key in Environment.GetEnvironmentVariables().Keys)
            {
                env[key.ToString()] = Environment.GetEnvironmentVariable(key.ToString());
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
