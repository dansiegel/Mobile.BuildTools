using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models;

namespace Mobile.BuildTools.Utils
{
    public static class ImageSearchUtil
    {
        public static IEnumerable<string> GetSearchPaths(BuildToolsConfig config, Platform platform, string buildConfiguration, string buildToolsConfigPath, string additionalSearchPaths = null, bool? ignoreDefaultSearchPaths = null, ILog logger = null)
        {
            if (string.IsNullOrEmpty(buildToolsConfigPath))
                return Array.Empty<string>();

            if (File.Exists(buildToolsConfigPath) || (!Directory.Exists(buildToolsConfigPath) && Path.HasExtension(buildToolsConfigPath)))
            {
                buildToolsConfigPath = new FileInfo(buildToolsConfigPath).DirectoryName;
            }

            var searchPaths = new List<string>();
            var imageConfig = config?.Images;
            var cliSearchPaths = !string.IsNullOrEmpty(additionalSearchPaths) ? additionalSearchPaths.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();
            if (cliSearchPaths.Any())
            {
                searchPaths.AddRange(additionalSearchPaths.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => GetSearchPath(x, buildToolsConfigPath)));
            }

            if (cliSearchPaths.Any() && ignoreDefaultSearchPaths.HasValue && ignoreDefaultSearchPaths.Value)
            {
                return searchPaths.Distinct();
            }


            if (imageConfig?.Directories?.Any() ?? false)
            {
                searchPaths.AddRange(imageConfig.Directories.Select(x => GetSearchPath(x, buildToolsConfigPath)));
            }

            var monoandroidKey = GetKey(imageConfig?.ConditionalDirectories?.Keys, "monoandroid", "android", "droid");
            var xamariniOSKey = GetKey(imageConfig?.ConditionalDirectories?.Keys, "xamarin.ios", "xamarinios", "ios", "apple");
            var xamarinMacKey = GetKey(imageConfig?.ConditionalDirectories?.Keys, "maccatalyst", "macos", "xamarin.mac", "xamarinmac", "mac", "apple");
            var xamarinTVOSKey = GetKey(imageConfig?.ConditionalDirectories?.Keys, "xamarin.tvos", "xamarintvos", "tvos", "apple");

            var windowsKey = GetKey(imageConfig?.ConditionalDirectories?.Keys, "windows", "winui", "uwp");
            var webAssemblyKey = GetKey(imageConfig?.ConditionalDirectories?.Keys, "browserwasm", "wasm", "webassembly");
            var platformKeys = new[] { monoandroidKey, xamariniOSKey, xamarinMacKey, xamarinTVOSKey, windowsKey, webAssemblyKey }.Where(x => x != null);

            switch (platform)
            {
                case Platform.Android:
                    if (!string.IsNullOrEmpty(monoandroidKey))
                        searchPaths.AddRange(imageConfig.ConditionalDirectories[monoandroidKey].Select(x => GetSearchPath(x, buildToolsConfigPath)));
                    break;
                case Platform.iOS:
                    if (!string.IsNullOrEmpty(xamariniOSKey))
                        searchPaths.AddRange(imageConfig.ConditionalDirectories[xamariniOSKey].Select(x => GetSearchPath(x, buildToolsConfigPath)));
                    break;
                case Platform.macOS:
                    if (!string.IsNullOrEmpty(xamarinMacKey))
                        searchPaths.AddRange(imageConfig.ConditionalDirectories[xamarinMacKey].Select(x => GetSearchPath(x, buildToolsConfigPath)));
                    break;
                case Platform.Windows:
                case Platform.UWP:
                    if (!string.IsNullOrEmpty(windowsKey))
                        searchPaths.AddRange(imageConfig.ConditionalDirectories[windowsKey].Select(x => GetSearchPath(x, buildToolsConfigPath)));
                    break;
                case Platform.WebAssembly:
                    if (!string.IsNullOrEmpty(webAssemblyKey))
                        searchPaths.AddRange(imageConfig.ConditionalDirectories[webAssemblyKey].Select(x => GetSearchPath(x, buildToolsConfigPath)));
                    break;
                case Platform.TVOS:
                    if (!string.IsNullOrEmpty(xamarinTVOSKey))
                        searchPaths.AddRange(imageConfig.ConditionalDirectories[xamarinTVOSKey].Select(x => GetSearchPath(x, buildToolsConfigPath)));
                    break;
            }

            // TODO: Make this even smarter with conditions like `Release || Store`... perhaps we also should consider evaluating the defined constants.
            var keys = imageConfig?.ConditionalDirectories?.Keys.Where(k => IsValidConditionalDirectory(k, platformKeys, buildConfiguration)) ?? Array.Empty<string>();
            foreach (var validCondition in keys)
            {
                searchPaths.AddRange(imageConfig.ConditionalDirectories[validCondition].Select(x => GetSearchPath(x, buildToolsConfigPath)));
            }

            var outputPaths = new List<string>(searchPaths.Distinct());
            foreach(var path in searchPaths)
            {
                if(string.IsNullOrEmpty(path))
                {
                    outputPaths.Remove(path);
                    continue;
                }

                if (Directory.Exists(path))
                    continue;

                logger?.LogWarning($"No directory was found at the path '{path}'. Any images that should have been in this path will not be processed.");
                outputPaths.Remove(path);
            }

            return outputPaths;
        }

        private static bool IsValidConditionalDirectory(string condition, IEnumerable<string> platformKeys, string buildConfiguration)
        {
            if (platformKeys.Any(k => k.Equals(condition, StringComparison.InvariantCultureIgnoreCase)))
                return false;

            if (condition.Equals(buildConfiguration, StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (!string.IsNullOrEmpty(condition) && condition[0] == '!')
                return !condition.Substring(1).Equals(buildConfiguration, StringComparison.InvariantCultureIgnoreCase);

            return false;
        }

        private static string GetKey(IEnumerable<string> conditionalKeys, params string[] possibleNames)
        {
            if (conditionalKeys is null) return null;

            foreach (var name in possibleNames)
            {
                var key = conditionalKeys.FirstOrDefault(x => x.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (!string.IsNullOrEmpty(key))
                    return key;
            }

            return null;
        }

        private static string GetSearchPath(string directory, string buildToolsConfigPath)
        {
            if (string.IsNullOrWhiteSpace(directory))
                return null;
            if (Path.IsPathRooted(directory))
                return directory;
            if (Uri.TryCreate(directory, UriKind.Absolute, out var result) && result.IsFile)
                return result.LocalPath;

            var sanitizedDirectoryPath = Path.Combine(directory.Split('/', '\\'));
            return Path.Combine(buildToolsConfigPath, sanitizedDirectoryPath);
        }
    }
}
