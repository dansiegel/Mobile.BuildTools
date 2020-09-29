using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if !SCHEMAGENERATOR
using Microsoft.Build.Framework;
using Mobile.BuildTools.Build;
#endif
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Models.Secrets;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mobile.BuildTools.Utils
{
    public static partial class ConfigHelper
    {
#if !SCHEMAGENERATOR
        public static BuildToolsConfig GetConfig(ITaskItem item) =>
            GetConfig(item.ItemSpec);
#endif

        public static void SaveConfig(BuildToolsConfig config, string path)
        {
            var filePath = GetConfigFilePath(path);
            var json = JsonConvert.SerializeObject(config, GetSerializerSettings());
            lock(lockObject)
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
#if !SCHEMAGENERATOR
                    Behavior = VersionBehavior.PreferBuildNumber,
                    Environment = VersionEnvironment.All,
#endif
                    VersionOffset = 0
                },
                Css = new XamarinCss
                {
                    Minify = false,
                    BundleScss = false
                },
                Images = new ImageResize
                {
                    ConditionalDirectories = null,
                    Directories = null
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
                    CreateInRoot = true,
                    FileName = "ReleaseNotes.txt",
                    MaxCommit = 10,
                    MaxDays = 7
                }
            };

#if !SCHEMAGENERATOR
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
#endif

            SaveConfig(config, path);

#if !DEBUG // Do not generate .gitignore for local debug builds
            var requiredContents = @"# Mobile.BuildTools
secrets.json
secrets.*.json
";
            var gitignoreFile = Path.Combine(path, ".gitignore");
            if (File.Exists(gitignoreFile))
            {
                if(!File.ReadAllText(gitignoreFile).Contains(Constants.SecretsJsonFileName))
                {
                    File.AppendAllText(gitignoreFile, $"\n\n{requiredContents}");
                }
            }
            else
            {
                File.WriteAllText(gitignoreFile, requiredContents);
            }
#endif
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

#if !SCHEMAGENERATOR
        internal static SecretsConfig GetSecretsConfig(IBuildConfiguration buildConfiguration) =>
            GetSecretsConfig(buildConfiguration.ProjectName, buildConfiguration.ProjectDirectory, buildConfiguration.Configuration);
#endif
        public static SecretsConfig GetSecretsConfig(string projectName, string projectDir, BuildToolsConfig config)
        {
            var configPath = Path.Combine(projectDir, Constants.SecretsConfigFileName);
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<SecretsConfig>(json);
            }

            if (config.ProjectSecrets != null && config.ProjectSecrets.Any(x => x.Key == projectName))
                return config.ProjectSecrets.First(x => x.Key == projectName).Value;

            return new SecretsConfig { Disable = true };
        }
    }
}
