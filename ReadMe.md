# Build Tools

There is a lot of talk these days about DevOps. One of the problems with DevOps is that it can be really challenging. Far too many companies suffer from reliance on poor practices that their Development teams know need to be fixed. Today we have a variety of Build Systems that are at our disposal and we no longer need to rely on such poor practices. Mobile.BuildTools can help turn your run of the mill project into a streamlined DevOps masterpiece.

Mobile.BuildTools is split into two basic categories:

1. Understanding your environment.
  After installing Mobile.BuildTools you will immediately have access to Build Properties that you can utilize in your own custom Build Tasks to determine information about both your Build Host and the Project MSBuild is about to compile.
1. Protecting Application Secrets
  Most applications these days have some sort of Client Id or Backend Uri that should be excluded from Source Control. It's easy to understand why a Client Id doesn't belong in Source Control, but protecting things like a Backend Uri

## Background

As part of my frustration at how challenging it was to go from File -> New Solution to a base project that was ready to put into a DevOps pipeline, I set out to create the Prism QuickStart Templates. Part of the templates included many of the features you see in the Mobile.BuildTools. As time went on I realized the need to decouple the tools from the template so as new features were added, or bugs fixed it could be more easily added.

## Support

If these projects have helped you reduce time to develop and made your app better, please consider donating to help me continue to support the community for free.

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.me/dansiegel)

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

**Notes:**

\* `IsOSX` and `IsLinux` can not be determined as true when the executing MSBuildRuntimeType is `Mono`.

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


[PrismNuGetShield]: https://img.shields.io/nuget/vpre/Prism.MFractor.Config.svg
[QuickStartNuGetShield]: https://img.shields.io/nuget/vpre/Prism.QuickStart.MFractor.Config.svg
[PrismNuGet]: https://www.nuget.org/packages/Prism.MFractor.Config/
[QuickStartNuGet]: https://www.nuget.org/packages/Prism.QuickStart.MFractor.Config/
[BuildToolsNuGet]: https://www.nuget.org/packages/Mobile.BuildTools/
[BuildToolsNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.svg
