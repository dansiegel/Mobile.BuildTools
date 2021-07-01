using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Configuration;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks
{
    public class GoogleTask : BuildToolsTaskBase
    {
        private List<ITaskItem> _outputs = new List<ITaskItem>();
        [Output]
        public ITaskItem[] GoogleOutput => _outputs.ToArray();

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            if (config.Platform == Platform.Android)
            {
                GetGoogleServicesJson(config);
            }
            else if (config.Platform == Platform.iOS)
            {
                GetGoogleInfoPlist(config);
            }
        }

        private void GetGoogleServicesJson(IBuildConfiguration buildConfig)
        {
            var path = GetResourcePath(buildConfig.Configuration.Google?.ServicesJson, buildConfig.IntermediateOutputPath, "google-services.json");

            if (string.IsNullOrEmpty(path))
                return;

            // Output to GoogleServicesJson Include="path"
            var item = new TaskItem(path);
            item.SetMetadata("LogicalName", "google-services.json");
            _outputs.Add(item);
        }

        private void GetGoogleInfoPlist(IBuildConfiguration buildConfig)
        {
            var path = GetResourcePath(buildConfig.Configuration.Google?.InfoPlist, buildConfig.IntermediateOutputPath, "GoogleService-Info.plist");

            if (string.IsNullOrEmpty(path))
                return;

            // Output to BundleResource Include="path"
            var item = new TaskItem(path);
            item.SetMetadata("LogicalName", "GoogleService-Info.plist");
            _outputs.Add(item);
        }

        internal static string GetResourcePath(string environmentVariable, string intermediateOutputPath, string logicalName)
        {
            if (string.IsNullOrEmpty(environmentVariable))
                return null;

            var variables = Environment.GetEnvironmentVariables().Keys.Cast<string>();
            var variableName = variables.FirstOrDefault(x => x.Equals(environmentVariable, StringComparison.InvariantCultureIgnoreCase));

            if (string.IsNullOrEmpty(variableName))
                return null;

            var value = Environment.GetEnvironmentVariable(variableName);
            try
            {
                if (File.Exists(value))
                    return new FileInfo(value).FullName;
            }
            catch
            {
                // Suppress errors
            }

            var directory = new DirectoryInfo(Path.Combine(intermediateOutputPath, "google-services"));
            directory.Create();
            var path = Path.Combine(directory.FullName, logicalName);
            File.WriteAllText(path, value);
            return path;
        }
    }
}
