# MSBuild Properties

The Mobile.BuildTools additionally provides a number of MSBuild Properties to further assist advanced developers in creating advanced build pipelines.

| PropertyName | Description |
|:------------:|:-----------:|
| IsWindows | |
| IsUnix | Indicates that you are running on a Linux or macOS build agent |
| PowerShellExe | returns the default path for the exe |
| IsAndroidProject | Indicates the current Target Framework is MonoAndroid |
| IsiOSProject | Indicates the current Target Framework is Xamarin.iOS |
| IsUWPProject | Indicates the current Target Framework is UAP |
| IsMacOSProject | Indicates the current Target Framework is Xamarin.Mac |
| IsTizenProject | Indicates the current Target Framework is Tizen |
| BuildToolsArtifactOutputPath | Will default to the Solution Directory in the `App` folder. In Azure DevOps it will default to the Build.ArtifactStagingDirectory again in the App folder. |
| IsAppCenter | Indicates the current build host is an App Center build agent |
| IsAzureDevOps | Indicates the current build host is an Azure DevOps build agent. |
| IsAppVeyor | Indicates the current build host is an AppVeyor build agent. |
| IsBitBucket | Indicates the current build host is a BitBucket build agent. |
| IsGitHubActions | Indicates the current build host is an GitHub Actions build agent. |
| IsJenkins | Indicates the current build host is a Jenkins build agent. |
| IsTeamCity | Indicates the current build host is a Team City build agent. |
| IsBuildHost | If any of the above CI Platforms return true this will indicate true as well. |
