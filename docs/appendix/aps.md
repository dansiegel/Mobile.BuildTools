# Apple Push Notifications

When using Push Notifications on iOS you must be sure to set the `aps-environment` from `development` to `production` before submitting to the App Store. The Mobile.BuildTools will handle this for you easily at build.

!!! danger "Critical Note"
    While this was originally slated for v2.0, this will not be done until 2.1.

## From the Build Definition

You can accomplish this easily by updating your build definition to pass additional MSBuild arguments with the value `/p:APSProductionEnvironment=true` and the Mobile.BuildTools will automatically update the `aps-environment` for you.

```yaml
- task: XamariniOS@2
  inputs:
    solutionFile: '**/*.sln'
    configuration: 'Store'
    packageApp: true
    runNugetRestore: false
    args: '/p:APSProductionEnvironment=true'
```

## From the MSBuild Properties

You can alternatively do this through any Directory.Build.props or in the csproj of your iOS project by adding the following:

```xml
<PropertyGroup Condition=" $(Configuration) == 'Store' ">
  <APSProductionEnvironment>true</APSProductionEnvironment>
</PropertyGroup>
```