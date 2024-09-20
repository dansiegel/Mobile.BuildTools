using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mobile.BuildTools.Models;

namespace Mobile.BuildTools.Utils
{
    public static partial class ConfigHelper
    {
        private static readonly object lockObject = new object();
        private static readonly object gitIgnoreLockObject = new object();

#if !ANALYZERS
        public static string GetConfigurationPath(string searchDirectory, string slnDir = null)
        {
            if (string.IsNullOrEmpty(searchDirectory)) return null;

            if (Path.GetFileName(searchDirectory) == Constants.BuildToolsConfigFileName)
            {
                return GetConfigurationPath(Path.GetDirectoryName(searchDirectory));
            }

            var rootPath = Path.GetPathRoot(searchDirectory);
            var configPath = new FileInfo(Path.Combine(searchDirectory, Constants.BuildToolsConfigFileName));
            if (configPath.Exists)
            {
                return configPath.FullName;
            }
            else if (configPath.Directory.EnumerateFiles("*.sln").Any() ||
                configPath.Directory.EnumerateDirectories().Any(x => x.Name == ".git"))
            {
                return configPath.FullName;
            }

            if (searchDirectory == rootPath)
            {
                var outputDir = string.IsNullOrEmpty(slnDir) ? searchDirectory : slnDir;
                SaveDefaultConfig(outputDir);
                return outputDir;
            }

            var parentDirectory = Directory.GetParent(searchDirectory);
            return GetConfigurationPath(parentDirectory.FullName, slnDir ?? searchDirectory);
        }

        public static bool Exists(string path) =>
            File.Exists(GetConfigFilePath(path));

        public static bool Exists(string path, out string filePath)
        {
            filePath = GetConfigFilePath(path);
            return File.Exists(filePath);
        }

        public static BuildToolsConfig GetConfig(string path, bool skipActivation = false)
        {
            var filePath = GetConfigurationPath(path);
            var configurationDirectoryPath = filePath;
            if (Path.GetFileName(filePath) != Constants.BuildToolsConfigFileName)
            {
                filePath = Path.Combine(filePath, Constants.BuildToolsConfigFileName);
            }

            if (!Exists(configurationDirectoryPath))
            {
                SaveDefaultConfig(configurationDirectoryPath);
            }

            var json = string.Empty;
            lock(lockObject)
            {
                json = File.ReadAllText(filePath);
            }

            var config = JsonSerializer.Deserialize<BuildToolsConfig>(json, GetSerializerSettings());
            if (skipActivation)
                return config;

            var props = typeof(BuildToolsConfig).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.PropertyType != typeof(bool) && x.PropertyType != typeof(string));

            foreach(var prop in props)
            {
                if(prop.GetValue(config) is null)
                {
                    var newInstance = Activator.CreateInstance(prop.PropertyType);
                    prop.SetValue(config, newInstance);
                }
            }

            return config;
        }

        private static string GetConfigFilePath(string path)
        {
            if (Path.GetFileName(path) == Constants.BuildToolsConfigFileName)
            {
                return path;
            }

            // Should only ever be used for tests.
            if (Path.HasExtension(path))
            {
                if (Path.GetExtension(path) == ".json")
                    return path;

                path = new FileInfo(path).DirectoryName;
            }

            return Path.Combine(path, Constants.BuildToolsConfigFileName);
        }
#endif

        public static JsonSerializerOptions GetSerializerSettings()
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true
            };

            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }

        private static readonly Dictionary<string, string> conditionalDefaults = new()
        {
            { "Debug", "Debug" },
            { "Release", "Release" },
            { "Store", "Store" },
            { "Ad-Hoc", "Ad-Hoc" },
            { "MonoAndroid", "MonoAndroid" },
            { "Xamarin.iOS", "Xamarin.iOS" },
            { "Android", "MonoAndroid" },
            { "iOS", "Xamarin.iOS" }
        };
    }
}
