<p align="center"><img src="logo/horizontal.svg" alt="Mobile.BuildTools" height="200px"></p>

# Build Tools

There is a lot of talk these days about DevOps. One of the problems with DevOps is that it can be really challenging. Far too many companies suffer from reliance on poor practices that their Development teams know need to be fixed. Today we have a variety of Build Systems that are at our disposal and we no longer need to rely on such poor practices. Mobile.BuildTools can help turn your run of the mill project into a streamlined DevOps masterpiece.

Mobile.BuildTools is split into the following categories:

1. Understanding your environment.
  After installing Mobile.BuildTools you will immediately have access to Build Properties that you can utilize in your own custom Build Tasks to determine information about both your Build Host and the Project MSBuild is about to compile.
1. Protecting Application Secrets
  Most applications these days have some sort of Client Id or Backend Uri that should be excluded from Source Control. It's easy to understand why a Client Id doesn't belong in Source Control, but protecting things like a Backend Uri
1. Enhancing the DevOps Experience
  There are certain tasks that can be painful such as automatically increasing the build version without having to check in a change to source control

## Background

As part of my frustration at how challenging it was to go from File -> New Solution to a base project that was ready to put into a DevOps pipeline, I set out to create the Prism QuickStart Templates. Part of the templates included many of the features you see in the Mobile.BuildTools. As time went on I realized the need to decouple the tools from the template so as new features were added, or bugs fixed it could be more easily added.

## Support

If these projects have helped you reduce time to develop and made your app better, please consider donating to help me continue to support the community for free.

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.me/dansiegel)

## Samples

- [AppCenter.DemoApp](https://github.com/dansiegel/AppCenter.DemoApp) - Sample Xamarin Forms app that protects the Info.plist &amp; AndroidManifest.xml, injects the AppCenter App Secrets, and automatically increments the app version on each build using timestamps locally and the Build Id when on built AppCenter.

## Prism MFractor Config

As part of the DevOps tooling, you can now easily include an MFractor config to enable MFractor to better utilize Prism naming conventions. If you have modified your project structure you can always override individual settings by adding your own MFractor config. More information can be found in the [MFractor Docs](http://docs.mfractor.com/). Configurations are available both for generic Prism applications and those using the Prism QuickStart Templates.

| MFractor Config | NuGet |
| --------------- | ----- |
| [Prism.MFractor.Config][PrismNuGet] | [![PrismNuGetShield]][PrismNuGet] |
| [Prism.QuickStart.MFractor.Config][QuickStartNuGet] | [![QuickStartNuGetShield]][QuickStartNuGet] |

## Mobile.BuildTools

| Package | NuGet |
| --------------- | ----- |
| [Mobile.BuildTools][BuildToolsNuGet] | [![BuildToolsNuGetShield]][BuildToolsNuGet] |

As part of the Build Tools, a number of Build Properties are added to better assist your DevOps pipeline by adding intelligence to the Build Process.

- IsWindows
- IsUnix (True on both macOS and Linux)
- *IsOSX
- *IsLinux
- IsiOSProject
- IsAndroidProject
- IsUWPProject
- IsMacOSProject
- IsTizenProject
- IsAppCenter
- IsAppVeyor
- IsJenkins
- IsTeamCity
- IsVSTS
- IsBuildHost

**Notes:**

\* `IsOSX` and `IsLinux` can not be determined as true when the executing MSBuildRuntimeType is `Mono`.

The Mobile.BuildTools are designed to keep Secrets Safe, while still allowing you to be able to debug. During the build outputs from Mobile.BuildTools will help you see that what keys were found and show you generally what was output. Because your secrets should be safe we default to sanitizing the output. This can be easily overriden if you need to debug your build. You can do this by updating either your `Directory.build.props` or csproj as follows:

```xml
<PropertyGroup>
  <!-- This will allow output of your actual secrets to the Build Log -->
  <MobileBuildToolsDebug>true</MobileBuildToolsDebug>
</PropertyGroup>
```

### Secrets

Modern Apps have a lot of secrets, or at the very least variables that may change based on an environment. Secrets could be something that really is sensitive and should be kept out of Source Control such as an OAuth Client Id, or it could be something such as the URL to use for the Backend which may change between Development, Staging, and Production. The Build Tools allow you to handle these secrets with ease in your application through the use of a JSON file.

```json
{
  "AppBackend": "https://backend.awesomeapp.com",
  "ClientId": "abc123",
  "IsSomethingTrue": true
}
```

By including a JSON File like the one shown above, the Secrets task will generate a class in your project like:

```cs
namespace YourApp.Helpers
{
    internal static class Secrets
    {
        internal const string AppBackend = "https://backend.awesomeapp.com";

        internal const string ClientId = "abc123";

        internal const bool IsSomethingTrue = true;
    }
}
```

#### v1.1 and beyond

Starting in v1.1 a new behavior has been introduced for the Secrets task. By default the Secrets class will be generated in the Intermediate Output Folder (`{ProjectPath}\obj\Secrets.cs`) rather than in the project itself (`{ProjectPath}\Helpers\Secrets.cs`). Since standard .gitignore files already ignore this folder you will only need to add an ignore for the secrets.json file. If you prefer to see the generated class and don't mind having the file ignored, all you need to do is create a file in the standard output path as shown above and it will update that file instead. When updating from 1.0, you will not automatically see any changes because you already have the Secrets.cs in your project. Deleting the file in your project will allow the Secrets task to change to the new behavior.

### Build Host Secrets

Obviously if we checked in a json file with our secrets it would negate the entire point of trying to secure our code base. The Build Tools include a Build Host Secrets task that executes prior to the Secrets Generation. This task will execute when the secrets json file does not exist in an attempt to generate the missing json file. This is designed to handle secrets across multiple projects. By default we assume you have a single shared project such as a .NET Standard library, and one or more platform projects like iOS, Android, UWP, macOS, & Tizen. To override the secrets prefix for any project you simply need to provide a value for `BuildHostSecretPrefix`

| Platform | Secrets Prefix |
| -------- | -------------- |
| Android | DroidSecret_ |
| iOS | iOSSecret_ |
| UWP | UWPSecret_ |
| macOS | MacSecret_ |
| Tizen | TizenSecret_ |
| Default | Secret_ |

### Tokenized Android &amp; iOS Configurations

The build tools allow you to exclude app manifests from Source Control. To do this of course you will need to add the file(s) you wish to exclude in your .gitignore, then you will need to add a Tokenized Template that will be copied to your project iOS or Android Project. By default the Info.plist is handled on iOS and the AndroidManifest.xml is handled on Android. For all other platforms you must configure this on your own. You can customize this by configuring any of the following build variables

| Build Variable | Default Value | Platform |
| -------------- | ------------- | -------- |
| BuildResourcesDirectoryName | build | All |
| BuildResourcesDirRootPath | `SolutionDir`/`BuildResourcesDirectoryName` | All |
| ManifestVariablePrefix | Manifest_ | All |
| ManifestToken | $$ | All |
| InfoPlistTemplateName | BuildTemplateInfo.plist | iOS |
| BuildTemplateInfoPlistPath | `BuildResourcesDirRootPath`/`InfoPlistTemplateName` | iOS |
| InfoPlistPath | `MSBuildProjectDirectory`/Info.plist | iOS |
| TemplateManifestPath | `BuildTemplateInfoPlistPath` | iOS |
| ManifestDestinationPath | `InfoPlistPath` | iOS |
| AndroidManifestTemplateName | AndroidTemplateManifest.xml | Android |
| BuildTemplateAndroidManifestPath | `BuildResourcesDirRootPath`/`AndroidManifestTemplateName` | Android |
| AndroidManifestPath | `MSBuildProjectDirectory`/Properties/AndroidManifest.xml | Android |
| ManifestDestinationPath | `BuildTemplateAndroidManifestPath` | Android |
| TemplateManifestPath | `AndroidManifestPath` | Android |
| ManifestDestinationPath | null | All Others |
| TemplateManifestPath | null | All Others |

Putting this all together say that we want to use Mobile Center for Push Notifications and Azure Active Directory with the Microsoft.Identity.Client. This would then require that we expose the Mobile Center App Id and our Azure Active Directory App Client Id in our Info.plist.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>CFBundleDisplayName</key>
  <string>Awesome App</string>
  <key>CFBundleURLTypes</key>
  <array>
    <dict>
      <key>CFBundleTypeRole</key>
      <string>Editor</string>
      <key>CFBundleURLName</key>
      <string>com.prismlib.awesomeapp</string>
      <key>CFBundleURLSchemes</key>
      <array>
        <string>msal$$AADClientId$$</string>
      </array>
    </dict>
    <dict>
      <key>CFBundleURLSchemes</key>
      <array>
        <string>mobilecenter-$$MobileCenterSecret$$</string>
      </array>
    </dict>
  </array>
</dict>
</plist>
```

As you can see from this sample we can protect our client id's by having a Tokenized version of our Info.plist checked into Source Control. We then simply need to add Environment Variables to our CI Build Definition like:

| Key | Value |
| --- | ----- |
| Manifest_AADClientId | 00000000-0000-0000-0000-000000000000 |
| Manifest_MobileCenterSecret | 11111111-1111-1111-1111-111111111111 |

### Automatic Build Versioning

The Mobile Build tools supports automatically incrementing the Build Version for Xamarin iOS and Xamarin Android projects. This can work EVERYWHERE including your local desktop/laptop. This is however an entirely Opt-In feature!

Automatic Build Versioning supports the following Versioning Environments:

| Environment | Description |
| ----------- | ----------- |
| All | Versioning will occur on every build. |
| BuildHost | Versioning will only occur if a \*Supported Build Host is detected. |
| Local | Versioning will only occur if a \*Supported Build Host is not detected. |

Automatic Build Versioning supports the following `Behavior`'s:

| Behavior | Description |
| -------- | ----------- |
| Off | Automatic Versioning is Disabled |
| PreferBuildNumber | When running on a \*Supported Build Host it will use the Build Number, otherwise it will use the current timestamp |
| Timestamp | Automatic Versioning will use the timestamp for the build |

\* Supported Build Hosts:

  - AppCenter
  - AppVeyor
  - Jenkins
  - TeamCity
  - VSTS

#### Configuring Automatic Versioning

To enable Automatic Versioning you will need to open your iOS or Android project edit the main `PropertyGroup`.

| Property | Default Value | Notes |
| -------- | ------------- | ----- |
| AutomaticVersionOffset | 0 | This value will be added to your Version Number |
| AutomaticVersionEnvironment | All | By default this will run everywhere |
| AutomaticVersionBehavior | n/a | The property must be set to `PreferBuildNumber` or `Timestamp` to enable the build task. |

```xml
<PropertyGroup>
  <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
  <Platform Condition=" '$(Platform)' == '' ">iPhoneSimulator</Platform>
  <ProductVersion>8.0.30703</ProductVersion>
  <SchemaVersion>2.0</SchemaVersion>
  <ProjectGuid>{99D11950-C303-4761-8045-4BCEEACCB226}</ProjectGuid>
  <ProjectTypeGuids>{FEACFBD2-3405-455C-9665-78FE426C6842};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
  <OutputType>Exe</OutputType>
  <RootNamespace>App1.iOS</RootNamespace>
  <IPhoneResourcePrefix>Resources</IPhoneResourcePrefix>
  <AssemblyName>App1.iOS</AssemblyName>
  <NuGetPackageImportStamp>
  </NuGetPackageImportStamp>
  <!-- Enable Automatic Versioning -->
  <AutomaticVersionBehavior>PreferBuildNumber</AutomaticVersionBehavior>
</PropertyGroup>
```

### App Package (Artifacts) Copy

It can sometimes be confusing as to where you generated App Package is after you build. Mobile.BuildTools helps you with this by automatically copying the generated APK, IPA and dSYM to an artifacts folder under the solution directory. It does this with some intelligence by disabling the copy on AppCenter since there is no need, and it will copy to `Build.ArtifactStagingDirectory` allowing you to more easily discover and consume the outputs when built on VSTS.

| Property | Default |
| -------- | ------- |
| BuildToolsArtifactOutputPath | `{Solution Dir}/Artifacts` |
| DisableBuildToolsArtifactCopy | `false` |

### SCSS For Xamarin.Forms 3.0

CSS support in Xamarin.Forms is the most revolutionary change to Styling XAML. CSS though is traditionally problematic on larger projects as it can quickly become hard to maintain, and error prone as it lacks reusability of common values which could include setting various properties or reusing the same color from one element to the next. With SCSS you gain the ability to break your stylesheets into logical reusable chunks and you gain the ability to define variables and functions for creating your styles. The Mobile.BuildTools now supports Xamarin.Forms CSS generation as part of the build process.

**NOTE** The Xamarin.Forms CSS spec does not generate valid CSS and as a result SCSS will not support the use of ^. 

Valid Xamarin.Forms CSS

```css
^button {
  background-color: transparent;
}

.primary ^button {
  background-color: #78909c;
}
```
The Mobile.BuildTools will post process your SCSS to generate valid CSS for Xamarin.Forms when using the selectors `any` or `all`.

Valid SCSS used by the Mobile.BuildTools

```css
button:any {
  background-color: transparent;
}

.primary button:all {
  background-color: #78909c;
}
```

To get started, simply add any scss format stylesheets you want to your project and make sure that the build action is set to `None`. The Mobile.BuildTools will automatically detect them and generate a CSS file for each non-partial (anything not starting with an underscore). For more information on how to get started with SCSS see the [Getting Started Guide](https://sass-lang.com/guide) from LibSass.

[PrismNuGetShield]: https://img.shields.io/nuget/vpre/Prism.MFractor.Config.svg
[QuickStartNuGetShield]: https://img.shields.io/nuget/vpre/Prism.QuickStart.MFractor.Config.svg
[PrismNuGet]: https://www.nuget.org/packages/Prism.MFractor.Config/
[QuickStartNuGet]: https://www.nuget.org/packages/Prism.QuickStart.MFractor.Config/
[BuildToolsNuGet]: https://www.nuget.org/packages/Mobile.BuildTools/
[BuildToolsNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.svg
