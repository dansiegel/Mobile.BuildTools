<p align="center"><img src="logo/horizontal.svg" alt="Mobile.BuildTools" height="200px"></p>

# Build Tools

There is a lot of talk these days about DevOps. One of the problems with DevOps is that it can be really challenging. Far too many companies suffer from reliance on poor practices that their Development teams know need to be fixed. Today we have a variety of Build Systems that are at our disposal and we no longer need to rely on such poor practices. Mobile.BuildTools can help turn your run of the mill project into a streamlined DevOps masterpiece. Best of all because the Mobile.BuildTools simply provide new targets for MSBuild it works absolutely EVERYWHERE that MSBuild itself is installed!

#### Background

As part of my frustration at how challenging it was to go from File -> New Solution to a base project that was ready to put into a DevOps pipeline, I set out to create the Prism QuickStart Templates. Part of the templates included many of the features you see in the Mobile.BuildTools. As time went on I realized the need to decouple the tools from the template so as new features were added, or bugs fixed it could be more easily added.

## Support

If this project helped you reduce time to develop and made your app better, please be sure to star the project & help support Dan.

[![GitHub Sponsors](https://github.blog/wp-content/uploads/2019/05/mona-heart-featured.png?fit=600%2C315)](https://xam.dev/sponsor-buildtools)


## Samples

NOTE: The sample apps are out of date and will be updated after Mobile.BuildTools 2.0 goes stable.

- [AppCenter.DemoApp](https://github.com/dansiegel/AppCenter.DemoApp) - Sample Xamarin Forms app that protects the Info.plist &amp; AndroidManifest.xml, injects the AppCenter App Secrets, and automatically increments the app version on each build using timestamps locally and the Build Id when on built AppCenter.

## Mobile.BuildTools

For more information on the various Build Tasks, and Properties please see the WIKI.

| Package | NuGet | MyGet |
| --------------- | ----- | ---- |
| [Mobile.BuildTools][BuildToolsNuGet] | [![BuildToolsNuGetShield]][BuildToolsNuGet] | [![BuildToolsMyGetShield]][BuildToolsMyGet] |
| [Mobile.BuildTools.Configuration][BuildToolsConfigNuGet] | [![BuildToolsConfigNuGetShield]][BuildToolsConfigNuGet] | [![BuildToolsConfigMyGetShield]][BuildToolsConfigMyGet] |

Want to consume the CI packages? Just be sure to add the following to your NuGet sources.

```
https://www.myget.org/F/dansiegel/api/v3/index.json
```

### What's New in v2.0?

Mobile.BuildTools v2 is almost entirely redesigned. Instead on simply supporting string, int's, doubles, and boolean values we now support every C# primitive plus Uri, DateTime, DateTimeOffset, TimeSpan, and Guid, as well as arrays.

Version 2 also deprecates a lot of the MSBuild property configurations that made people nervious with a preference on using easy to use json configuration files which we will autogenerate for you the first build and which will also add the schema url to ensure you get intellisense in your text editor. Configuration files for Mobile.BuildTools should be considered safe to check in, and in many ways help provide a lot of context for people who are cloning a clean copy of your project as you can now more easily determine what the structure of the secrets.json should be. We've also spent a lot of time working on the way we handle secrets in the AndroidManifest.xml and Info.plist.

#### Json Configurations

MSBuild confuses most people. Version 2 now takes the approach of something you're used to working with, JSON. We use it for pretty much everything. You should of course be sure to exclude the `secrets.json` in your `.gitignore`, but any configurations you may have are safe to check in. These configurations allow you to easily opt-out of targets or configure how you want the Secrets file to be generated.

#### Introducing app.config Support!

One of the exciting new features in v2.0 is support for the trusty and familiar app.config. This great feature is built on top of the fantastic work from [Chase Florell](https://github.com/chaseflorell). For those coming from a WPF or ASP.NET background this should feel very familiar and we fully support build time transformations allowing us to inject these assets into your app.

#### Image Processing

Image handling for mobile projects really sucks to say the least. Whether you want to get into the mud on whether or not you should checkin binary files to a git repo or not is on you. That said having to make 8 or more copies of the same image and then make sure they're in sync and you have the correct resolutions for iOS and Android is just a pain. The Mobile.BuildTools aims to make this experience easier by ensuring that you can now simply provide it search paths with optional configuration files for any image. So what are the benefits here? You can now keep your project lightweight only putting the files directly into it that you want. You can also get creative with conditionally included directories which may have a configuration file for an image in another directory.

[PrismNuGetShield]: https://img.shields.io/nuget/vpre/Prism.MFractor.Config.svg
[QuickStartNuGetShield]: https://img.shields.io/nuget/vpre/Prism.QuickStart.MFractor.Config.svg
[PrismNuGet]: https://www.nuget.org/packages/Prism.MFractor.Config/
[QuickStartNuGet]: https://www.nuget.org/packages/Prism.QuickStart.MFractor.Config/

[BuildToolsNuGet]: https://www.nuget.org/packages/Mobile.BuildTools/
[BuildToolsNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.svg
[BuildToolsMyGet]: https://www.myget.org/feed/dansiegel/package/nuget/Mobile.BuildTools/
[BuildToolsMyGetShield]: https://img.shields.io/myget/dansiegel/vpre/Mobile.BuildTools.svg

[BuildToolsConfigNuGet]: https://www.nuget.org/packages/Mobile.BuildTools.Configuration/
[BuildToolsConfigNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.Configuration.svg
[BuildToolsConfigMyGet]: https://www.myget.org/feed/dansiegel/package/nuget/Mobile.BuildTools.Configuration/
[BuildToolsConfigMyGetShield]: https://img.shields.io/myget/dansiegel/vpre/Mobile.BuildTools.Configuration.svg
