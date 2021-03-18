using System;
using System.Collections.Generic;
using System.IO;
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
            if (config.Platform == Platform.Android && !string.IsNullOrEmpty(config.Configuration.Google?.ServicesJson))
            {
                GetGoogleServicesJson(config);
            }
            else if (config.Platform == Platform.iOS && !string.IsNullOrEmpty(config.Configuration.Google?.InfoPlist))
            {
                GetGoogleInfoPlist(config);
            }
        }

        private void GetGoogleServicesJson(IBuildConfiguration buildConfig)
        {
            var json = Environment.GetEnvironmentVariable(buildConfig.Configuration.Google.ServicesJson);
            if (string.IsNullOrEmpty(json))
                return;

            var path = Path.Combine(buildConfig.IntermediateOutputPath, "aritchie-sucks", "google-services.json");
            File.WriteAllText(path, json);
            // Output to GoogleServicesJson Include="path"
            var item = new TaskItem(path);
            item.SetMetadata("LogicalName", "google-services.json");
            _outputs.Add(item);
        }

        private void GetGoogleInfoPlist(IBuildConfiguration buildConfig)
        {
            var json = Environment.GetEnvironmentVariable(buildConfig.Configuration.Google.InfoPlist);
            if (string.IsNullOrEmpty(json))
                return;

            var path = Path.Combine(buildConfig.IntermediateOutputPath, "aritchie-sucks", "GoogleService-Info.plist");
            File.WriteAllText(path, json);
            // Output to BundleResource Include="path"
            var item = new TaskItem(path);
            item.SetMetadata("LogicalName", "GoogleService-Info.plist");
            _outputs.Add(item);
        }
    }
}
