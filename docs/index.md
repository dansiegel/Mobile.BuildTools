# Getting Started

Mobile.BuildTools is a NuGet package that adds MSBuild targets to your build pipeline for application configuration, manifests, images, and other build tasks. For new applications, use .NET MAUI or Uno Platform and review the [modern .NET notes](maui.md). Platform-specific behavior depends on your target SDK and package version.

The Mobile.BuildTools are a collection of MSBuild Tasks that help make MSBuild smarter in handling the build process for CI/CD with Mobile Applications. The library was born from a desire to share build processes from one app to the next without having to copy and paste a bunch of build scripts each of which could easily end up out of date. Because the Mobile.BuildTools simply provides MSBuild Tasks, this adds nothing to the size of your application and if being used on a project that will be packed and shared, you can set the PackageReference's PrivateAssets to all.

!!! warning "NOTE"
    The docs on this site are specific to the Mobile.BuildTools v2.0. For those still using v1.4 please refer to the Wiki on GitHub.

## History

The Mobile.BuildTools started out several years ago from powershell scripts I found myself writing from one project to the next to help me decouple app secrets and configuration values from the code base making these instead build time dependencies. Over the years this grew from the scripts I kept re-writing to scripts that I included out of the box with the Prism Quickstart templates. These features had nothing to do with Prism however, and I started looking at how these could be re-used for any Xamarin application. With that the Mobile.BuildTools was born. Over the years it has grown and had so many features added that I ultimately realized I better start writing a docs site as it has a solution for pretty much every problem I encounter when developing mobile apps.

## Support

This project is maintained by Dan Siegel.

!!! note
    Enterprise Support is available through [AvantiPoint](https://avantipoint.com). Please reach out if you require support for your apps with the Mobile.BuildTools, Prism or general assistance with your .NET MAUI or Uno Platform apps.

## Latest NuGet's

| Package | NuGet |
| --------------- | ----- |
| [Mobile.BuildTools][BuildToolsNuGet] | [![Latest NuGet][BuildToolsNuGetShield]][BuildToolsNuGet] |
| [Mobile.BuildTools.Configuration][BuildToolsConfigNuGet] | [![Latest NuGet][BuildToolsConfigNuGetShield]][BuildToolsConfigNuGet] |

## Why Use the Mobile.BuildTools

The Mobile.BuildTools is designed to help you with a variety of tasks that make mobile app development easier and help you to follow best practices keeping app secrets as well as environment configurations out of your source control. Additionally the Mobile.BuildTools has a variety of helpers that can help you manage your image resources, white label apps, or generally make it easier to build your app for side by side installation for different environments like Dev, Stage, and Production.

## What does it do?

- Automatic app bundle copy to artifacts folder in the Solution directory
    - On iOS this copies your IPA + dSYM
    - On Android this copies your APK / AAB
- [Automatically update the app version](manifests/versioning.md)
    - Configurable for local, build host or both
    - Configurable to use timestamp or BuildId if it exists + user defined Offset
- [Tokenize your Info.plist / AndroidManifest.xml](manifests/index.md) (`$$SomeKey$$`)
    - Tokens replaced at build in obj to protect any against configuration values being checked in
- [Generate 'AppSettings' class at build that contains any configuration or application secrets](config/appsettings/index.md)
    - Supports all primitive data types + Uri, DateTime, DateTimeOffset, TimeStamp, & Guid
    - Any property can be made an array of values (useful for feature flags or OAuth scopes)
- [ConfigurationManager with app.config](config/app.config/index.md)
    - Optimized for Mobile with familiar Static API and Interface based Singleton
    - Offers Opt-In API for doing runtime transformations and bundling app.config's
    - App.config transformations at build
    - Optionally bundle all config's or config's with Non-Standard environment (i.e. not Debug, Release)
- [Simplified Image Handling (Android, iOS, macOS, tvOS)](images/index.md)
    - Ability to store single high resolution images in one or more directories
    - Ability to conditionally include images in directories based on the build target (i.e. iOS or Android)
    - Ability to conditionally include images in directories based on the build configuration (i.e. Debug or !Debug)
    - Ability to include images that may only be used as an overlay for another image.
    - Ability to Draw Banner on images (i.e. Dev, Debug, Free, Pro)
        - User controlled Text
        - User controlled Text color & Font from System Font or local font file
        - User control Banner color.. can include a single color for a solid look, or multiple colors for a gradient
    - Ability to generate additional outputs for a single input image
    - Ability add padding around an image
    - Ability to add a background color to a transparent image
    - Supports PNG & JPG file types
    - Support for SVG and Gif **(Planned)**
- [Release Notes generation](release-notes.md)
    - Customizable output based on latest commit messages
    - **(Planned)** Support for user templating and Flag based messages since last release/Git Tag... (i.e. `[Bug][iOS] Some bug got fixed`)

### Additional Notes

Some additional notes... the Mobile.BuildTools will help with some advanced scenarios like:

- Generating app bundles that have different ID's for different environments
- White Labeling
- Use the Secrets to generate Feature Flags
- Generate Free or Lite versions of your app along with a Pro version
- Using the image api you could have a single input image and generate each of the following
    - All of the outputs defined for the AppIcon iconset on iOS
    - A typical BundleResource 1x, 2x, 3x for using in your SplashScreen storyboard
    - A typical Drawable for you a splash screen activity (ldpi, mdpi, hdpi, xhdpi, xxhdpi, xxxhdpi)
    - The smaller icon file in the mipmap folders (ldpi, mdpi, hdpi, xhdpi, xxhdpi, xxxhdpi)
    - The larger launcher_foreground file in the mipmap folders (ldpi, mdpi, hdpi, xhdpi, xxhdpi, xxxhdpi)

[PrismNuGetShield]: https://img.shields.io/nuget/vpre/Prism.MFractor.Config.svg

[BuildToolsNuGet]: https://www.nuget.org/packages/Mobile.BuildTools/
[BuildToolsNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.svg

[BuildToolsConfigNuGet]: https://www.nuget.org/packages/Mobile.BuildTools.Configuration/
[BuildToolsConfigNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.Configuration.svg
