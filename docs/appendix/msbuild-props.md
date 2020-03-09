# MSBuild Properties

The Mobile.BuildTools additionally provides a number of MSBuild Properties to futher assist advanced developers in creating advanced build pipelines.

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
| IsAppCenter | Indicates that the AppCenter Build Id has been set |
| IsAzureDevOps | Indicates that the variable BUILD_BUILDNUMBER has been set. This may indicate true when building on certain other CI systems. |
| IsAppVeyor | Indicates that the AppVeyor Build Number has been set. |
| IsJenkins | Indicates that a Build Number has been set but there is no Team City Version |
| IsTeamCity | Indicates that a Build Number has been set and there is also a Team City Version set. |
| IsBuildHost | If any of the above CI Platforms return true this will indicate true as well. |