using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mobile.BuildTools.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mobile.BuildTools.Utils
{
    public static partial class ConfigHelper
    {
        private static readonly object lockObject = new object();

        public static string GetConfigurationPath(string searchDirectory, string slnDir = null)
        {
            if (string.IsNullOrEmpty(searchDirectory)) return null;

            var rootPath = Path.GetPathRoot(searchDirectory);
            var configPath = new FileInfo(Path.Combine(searchDirectory, "buildtools.json"));
            if (configPath.Exists)
            {
                return searchDirectory;
            }
            else if (configPath.Directory.EnumerateFiles("*.sln").Any() ||
                configPath.Directory.EnumerateDirectories().Any(x => x.Name == ".git"))
            {
                return configPath.Directory.FullName;
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

        public static BuildToolsConfig GetConfig(string path)
        {
            var filePath = GetConfigurationPath(path);
            if(File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
            {
                filePath = Path.Combine(filePath, "buildtools.json");
            }

            var json = string.Empty;
            lock(lockObject)
            {
                json = File.ReadAllText(filePath);
            }

            var config = JsonConvert.DeserializeObject<BuildToolsConfig>(json, GetSerializerSettings());

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

        public static JsonSerializerSettings GetSerializerSettings()
        {
            var serializer = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
            };
            serializer.Converters.Add(new StringEnumConverter());
            return serializer;
        }

        private static readonly Dictionary<string, string> conditionalDefaults = new Dictionary<string, string>
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
