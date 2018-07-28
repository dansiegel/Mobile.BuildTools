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

For more information on the various Build Tasks, and Properties please see the WIKI.

| Package | NuGet |
| --------------- | ----- |
| [Mobile.BuildTools][BuildToolsNuGet] | [![BuildToolsNuGetShield]][BuildToolsNuGet] |

### Upgrading to v2

It's important that builds have some consistancy. For that reason the Mobile.BuildTools will continue to operate much the same way. You shouldn't have to make ANY changes when upgrading from v1.x. Version 2.0 is all about making it even easier, and introduces some changes to help.

A large focus here is on handling your Manifests for iOS and Android. As a result the underlying build Targets around handling and Manifests have been completely rewritten to eliminate the need to maintain both a Templated Manifest and a working Manifest (for local development excluded from Source Control). For more information see what's new in v2.0 in the WIKI.

As part of the Build Tools, a number of Build Properties are added to better assist your DevOps pipeline by adding intelligence to the Build Process.



[PrismNuGetShield]: https://img.shields.io/nuget/vpre/Prism.MFractor.Config.svg
[QuickStartNuGetShield]: https://img.shields.io/nuget/vpre/Prism.QuickStart.MFractor.Config.svg
[PrismNuGet]: https://www.nuget.org/packages/Prism.MFractor.Config/
[QuickStartNuGet]: https://www.nuget.org/packages/Prism.QuickStart.MFractor.Config/
[BuildToolsNuGet]: https://www.nuget.org/packages/Mobile.BuildTools/
[BuildToolsNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.svg
