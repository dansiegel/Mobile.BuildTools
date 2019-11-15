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

If this project helped you reduce time to develop and made your app better, please be sure to star the project & help support Dan.

[![GitHub Sponsors](https://github.blog/wp-content/uploads/2019/05/mona-heart-featured.png?fit=600%2C315)](https://xam.dev/sponsor-buildtools)


## Samples

- [AppCenter.DemoApp](https://github.com/dansiegel/AppCenter.DemoApp) - Sample Xamarin Forms app that protects the Info.plist &amp; AndroidManifest.xml, injects the AppCenter App Secrets, and automatically increments the app version on each build using timestamps locally and the Build Id when on built AppCenter.

## Mobile.BuildTools

For more information on the various Build Tasks, and Properties please see the WIKI.

| Package | NuGet |
| --------------- | ----- |
| [Mobile.BuildTools][BuildToolsNuGet] | [![BuildToolsNuGetShield]][BuildToolsNuGet] |
| [Mobile.BuildTools.Configuration][BuildToolsConfigNuGet] | [![BuildToolsConfigNuGetShield]][BuildToolsConfigNuGet] |

### Upgrading to v2

It's important that builds have some consistancy. For that reason the Mobile.BuildTools will continue to operate much the same way. You shouldn't have to make ANY changes when upgrading from v1.x. Version 2.0 is all about making it even easier, and introduces some changes to help.

Much of the Mobile.BuildTools will continue to opperate much the same way that you're used to. However handling Manifests is now completely different! For v2.0 we now use the same tooling that the Xamarin Team uses internally to process your plists and Android Manifest. This means that we can seamlessly update your plist during the build process allowing you to tokenize your Manifests in place and inject secrets safely into them at build. These tasks are optimized so that your first build will handle the replacements and we won't touch it again unless there is a need to.

#### Introducing app.config Support!

One of the exciting new features in v2.0 is support for the trusty and familiar app.config. This great feature is built on top of the fantastic work from [Chase Florell](https://github.com/chaseflorell). For those coming from a WPF or ASP.NET background this should feel very familiar and we fully support build time transformations allowing us to inject these assets into your app.

[PrismNuGetShield]: https://img.shields.io/nuget/vpre/Prism.MFractor.Config.svg
[QuickStartNuGetShield]: https://img.shields.io/nuget/vpre/Prism.QuickStart.MFractor.Config.svg
[PrismNuGet]: https://www.nuget.org/packages/Prism.MFractor.Config/
[QuickStartNuGet]: https://www.nuget.org/packages/Prism.QuickStart.MFractor.Config/
[BuildToolsNuGet]: https://www.nuget.org/packages/Mobile.BuildTools/
[BuildToolsNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.svg

[BuildToolsConfigNuGet]: https://www.nuget.org/packages/Mobile.BuildTools.Configuration/
[BuildToolsConfigNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.Configuration.svg
