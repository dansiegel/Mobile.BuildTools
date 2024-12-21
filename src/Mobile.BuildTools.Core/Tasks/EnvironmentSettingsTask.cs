using System.Text.Json;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks;

public class EnvironmentSettingsTask : BuildToolsTaskBase
{
    [Required]
    public string RootNamespace { get; set; }

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
            Debug = config.Configuration.Debug,
            ProjectName = ProjectName,
            RootNamespace = RootNamespace,
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
            IsTravisCI = CIBuildEnvironmentUtils.IsTravisCI,
            BuildConfiguration = config.BuildConfiguration,
            TargetPlatform = config.Platform,
            GeneratedClasses = [],
            Environment = new Dictionary<string, string>()
        };

        if (config.Configuration.AppSettings is not null &&
            config.Configuration.AppSettings.TryGetValue(ProjectName, out var settings) &&
            settings.Any())
        {
            settings
                .GroupBy(c => c.FullyQualifiedClassName)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList()
                .ForEach(name => Log.LogError("Duplicate class name found in the {0} settings: {1}", ProjectName, name));

            if (Log.HasLoggedErrors)
            {
                return;
            }

            environment.GeneratedClasses = settings;
            var env = EnvironmentAnalyzer.GatherEnvironmentVariables(this);
            if (env.Count > 0)
            {
                environment.Environment = env;
            }
        }

        File.WriteAllText(outputPath, JsonSerializer.Serialize(environment, new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            WriteIndented = true
        }));
        EnvironmentSettings = [new TaskItem(outputPath)];
    }
}
