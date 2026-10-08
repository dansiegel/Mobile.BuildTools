# MSBuild Properties

These properties are evaluated by the build assets in your installed MBT version. Some legacy framework and host detection remains for compatibility; it does not guarantee support for retired targets or services. Current source also detects modern .NET platform identifiers.

| PropertyName | Description |
|:------------:|:-----------:|
| IsWindows | |
| IsUnix | Indicates that you are running on a Linux or macOS build agent |
| PowerShellExe | returns the default path for the exe |
| IsAndroidProject | Indicates an Android target (including the modern .NET platform in current source) |
| IsiOSProject | Indicates an iOS target (including the modern .NET platform in current source) |
| IsUWPProject | Indicates the current Target Framework is UAP |
| IsMacOSProject | Indicates a macOS target |
| IsTizenProject | Indicates the current Target Framework is Tizen |
| BuildToolsArtifactOutputPath | Will default to the Solution Directory in the `App` folder. In Azure DevOps it will default to the Build.ArtifactStagingDirectory again in the App folder. |
| IsAppCenter | Legacy App Center build-host detection |
| IsAzureDevOps | Indicates the current build host is an Azure DevOps build agent. |
| IsAppVeyor | Indicates the current build host is an AppVeyor build agent. |
| IsBitBucket | Indicates the current build host is a BitBucket build agent. |
| IsGitHubActions | Indicates the current build host is an GitHub Actions build agent. |
| IsJenkins | Indicates the current build host is a Jenkins build agent. |
| IsTeamCity | Indicates the current build host is a Team City build agent. |
| IsBuildHost | If any of the above CI Platforms return true this will indicate true as well. |
