using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
#if !ANALYZERS
using Mobile.BuildTools.Build;
#endif
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Models.Settings;

namespace Mobile.BuildTools.Utils
{
    public static partial class ConfigHelper
    {
#if !ANALYZERS
        public static void SaveConfig(BuildToolsConfig config, string path)
        {
            var filePath = GetConfigFilePath(path);
            var json = JsonSerializer.Serialize(config, GetSerializerSettings());
            lock (lockObject)
            {
                File.WriteAllText(filePath, json);
            }
        }

        public static void SaveDefaultConfig(string path)
        {
            var config = new BuildToolsConfig
            {
                ArtifactCopy = new ArtifactCopy { Disable = false },
                AppConfig = new AppConfig
                {
                    Strategy = AppConfigStrategy.TransformOnly
                },
                AutomaticVersioning = new AutomaticVersioning
                {
                    Behavior = VersionBehavior.PreferBuildNumber,
                    Environment = VersionEnvironment.All,
                    VersionOffset = 0
                },
                Css = new XamarinCss
                {
                    Minify = false,
                    BundleScss = false
                },
                Images = new ImageResize
                {
                    ConditionalDirectories = new Dictionary<string, IEnumerable<string>>
                    {
                        { "Debug", Array.Empty<string>() },
                        { "!Debug", Array.Empty<string>() },
                        { "iOS", Array.Empty<string>() },
                        { "Android", Array.Empty<string>() },
                    },
                    Directories = new List<string>()
                },
                Manifests = new TemplatedManifest
                {
                    Disable = false,
                    Token = "$",
                    MissingTokensAsErrors = false,
                    VariablePrefix = "Manifest_"
                },
                ReleaseNotes = new ReleaseNotesOptions
                {
                    Disable = false,
                    CharacterLimit = 250,
                    CreateInRoot = false,
                    FileName = "ReleaseNotes.txt",
                    MaxCommit = 10,
                    MaxDays = 7
                },
                Environment = new EnvironmentSettings
                {
                    Configuration = new Dictionary<string, Dictionary<string, string>>
                    {
                        { "Debug", new Dictionary<string, string>() }
                    },
                    Defaults = new Dictionary<string, string>()
                }
            };

            var imagesRootDir = Path.Combine(path, "Images");
            if (Directory.Exists(imagesRootDir))
            {
                if (Directory.GetDirectories(imagesRootDir).Length > 0)
                {
                    var allDirectories = Directory.GetDirectories(imagesRootDir).Select(x => GetRelativePath(x, path));
                    config.Images.Directories = allDirectories.Where(x => !conditionalDefaults.Any(d => x.Equals(d.Key, StringComparison.InvariantCultureIgnoreCase)))
                        .Select(x => Path.Combine("Images", x)).ToList();
                    config.Images.ConditionalDirectories = allDirectories.Where(x => conditionalDefaults.Any(d => x.Equals(d.Key, StringComparison.InvariantCultureIgnoreCase)))
                        .ToDictionary(x => conditionalDefaults[x], x => (IEnumerable<string>)new[] { Path.Combine("Images", x) });
                }
                else
                {
                    config.Images.Directories = new List<string> { GetRelativePath(imagesRootDir, path) };
                }
            }

            SaveConfig(config, path);

#if !DEBUG // Do not generate .gitignore for local debug builds
            var requiredContents = @"# Mobile.BuildTools
appsettings.json
appsettings.*.json
";
            var gitignoreFile = Path.Combine(Path.GetDirectoryName(path), ".gitignore");
            lock (gitIgnoreLockObject)
            {
                if (!File.Exists(gitignoreFile))
                {
                    File.WriteAllText(gitignoreFile, requiredContents);
                }
                else if (!File.ReadAllText(gitignoreFile).Contains(Constants.SecretsJsonFileName))
                {
                    WriteIfMissing(gitignoreFile, requiredContents);
                }
            }

#endif
        }

        private static void WriteIfMissing(string path, string requiredContents)
        {
            if (!File.ReadAllText(path).Contains(requiredContents))
            {
                File.AppendAllText(path, $"\n\n{requiredContents}");
            }
        }

        private static string GetRelativePath(string filespec, string folder)
        {
            var pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folder += Path.DirectorySeparatorChar;
            }
            var folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        public static IEnumerable<SettingsConfig> GetSettingsConfig(IBuildConfiguration buildConfiguration) =>
            GetSettingsConfig(buildConfiguration.ProjectName, buildConfiguration.Configuration);
#endif

        public static IEnumerable<SettingsConfig> GetSettingsConfig(string projectName, BuildToolsConfig config)
        {
            if (config.AppSettings != null && config.AppSettings.Any(x => x.Key == projectName))
                return config.AppSettings[projectName];

            return Array.Empty<SettingsConfig>();
        }
    }
}
