# Getting Started

The Mobile.BuildTools is an easy to use NuGet package that adds new MSBuild targets to your build pipeline. In essence it teaches MSBuild some new tricks to help make your DevOps easier and help you follow better practices while developing your application. The bulk of support is around Xamarin.Android and Xamarin.iOS and it will work regardless of whether you are using the native tooling, Xamarin.Forms, or Uno to create your UI.

The Mobile.BuildTools are a collection of MSBuild Tasks that help make MSBuild smarter in handling the build process for CI/CD with Mobile Applications. The library was born from a desire to share build processes from one app to the next without having to copy and paste a bunch of build scripts each of which could easily end up out of date. Because the Mobile.BuildTools simply provides MSBuild Tasks, this adds nothing to the size of your application and if being used on a project that will be packed and shared, you can set the PackageReference's PrivateAssets to all.

!!! warning "NOTE"
    The docs on this site are specific to the Mobile.BuildTools v2.0. For those still using v1.4 please refer to the Wiki on GitHub.

## Support

This project is maintained by Dan Siegel. If this project has helped you please consider sponsoring Dan on GitHub. Your contributions help make great open source projects possible.

[![GitHub Sponsors](https://github.blog/wp-content/uploads/2019/05/mona-heart-featured.png?fit=600%2C315)][sponsor]

## Latest NuGet's

| Package | NuGet | SponsorConnect |
| --------------- | ----- | ---- |
| [Mobile.BuildTools][BuildToolsNuGet] | [![Latest NuGet][BuildToolsNuGetShield]][BuildToolsNuGet] | [![Latest CI Package][BuildToolsSponsorConnectShield]][BuildToolsSponsorConnect] |
| [Mobile.BuildTools.Configuration][BuildToolsConfigNuGet] | [![Latest NuGet][BuildToolsConfigNuGetShield]][BuildToolsConfigNuGet] | [![Latest CI Package][BuildToolsConfigSponsorConnectShield]][BuildToolsConfigSponsorConnect] |

Want to consume the CI packages? Sign up as a [GitHub sponsor][sponsor] and you can access the Sponsor Connect private feed.

## What does it do?

- Automatic app bundle copy to artifacts folder in the Solution directory
    - On iOS this copies your IPA + dSYM
    - On Android this copies your APK / AAB
- [Automatically update the app version](manifests/versioning.md)
    - Configurable for local, build host or both
    - Configurable to use timestamp or BuildId if it exists + user defined Offset
- [Tokenize your Info.plist / AndroidManifest.xml](manifests/index.md) (`$$SomeKey$$`)
    - Tokens replaced at build in obj to protect any against configuration values being checked in
- [Support for SCSS files to generate Xamarin.Forms CSS files](scss-to-css/index.md)
- [Generate 'Secrets' class at build that contains any configuration or application secrets](config/secrets/basics.md)
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

[sponsor]: https://xam.dev/sponor-buildtools

[PrismNuGetShield]: https://img.shields.io/nuget/vpre/Prism.MFractor.Config.svg

[BuildToolsNuGet]: https://www.nuget.org/packages/Mobile.BuildTools/
[BuildToolsNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.svg
[BuildToolsSponsorConnect]: https://sponsorconnect.dev/nuget/package/Mobile.BuildTools/
[BuildToolsSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FMobile.BuildTools%2Fvpre

[BuildToolsConfigNuGet]: https://www.nuget.org/packages/Mobile.BuildTools.Configuration/
[BuildToolsConfigNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.Configuration.svg
[BuildToolsConfigSponsorConnect]: https://sponsorconnect.dev/nuget/package/Mobile.BuildTools.Configuration/
[BuildToolsConfigSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FMobile.BuildTools.Configuration%2Fvpre