﻿using System.Text.Json;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks;

public class EnvironmentSettingsTask : BuildToolsTaskBase
{
    [Output]
    public ITaskItem[] EnvironmentSettings { get; private set; } = [];
    internal override void ExecuteInternal(IBuildConfiguration config)
    {
        var outputPath = Path.Combine(IntermediateOutputPath, "Mobile.BuildTools", Constants.BuildToolsEnvironmentSettings);

        var fileInfo = new FileInfo(outputPath);
        if (!fileInfo.Directory.Exists)
        {
            fileInfo.Directory.Create();
        }

        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }

        var environment = new BuildEnvironment
        {
            BuildNumber = CIBuildEnvironmentUtils.BuildNumber,
            IsCI = CIBuildEnvironmentUtils.IsCI,
            IsAppCenter = CIBuildEnvironmentUtils.IsAppCenter,
            IsAppVeyor = CIBuildEnvironmentUtils.IsAppVeyor,
            IsAzureDevOps = CIBuildEnvironmentUtils.IsAzureDevOps,
            IsBitBucket = CIBuildEnvironmentUtils.IsBitBucket,
            IsBuildHost = CIBuildEnvironmentUtils.IsBuildHost,
            IsGitHubActions = CIBuildEnvironmentUtils.IsGitHubActions,
            IsJenkins = CIBuildEnvironmentUtils.IsJenkins,
            IsTeamCity = CIBuildEnvironmentUtils.IsTeamCity,
            IsTravisCI = CIBuildEnvironmentUtils.IsTravisCI
        };

        if (this is IBuildConfiguration buildConfiguration && buildConfiguration.Configuration.AppSettings is not null &&
            buildConfiguration.Configuration.AppSettings.TryGetValue(ProjectName, out var settings) && settings.Any())
        {
            var env = EnvironmentAnalyzer.GatherEnvironmentVariables(this);
            if (env.Count > 0)
            {
                environment.Environment = env;
            }
        }

        File.WriteAllText(outputPath, JsonSerializer.Serialize(environment, new JsonSerializerOptions(JsonSerializerDefaults.General)));
        EnvironmentSettings = [new TaskItem(outputPath)];
    }
}
