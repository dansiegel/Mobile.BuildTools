<p align="center"><img src="logo/horizontal.svg" alt="Mobile.BuildTools" height="150px"></p>

# Build Tools

There is a lot of talk these days about DevOps. One of the problems with DevOps is that it can be really challenging. Far too many companies suffer from reliance on poor practices that their Development teams know need to be fixed. Today we have a variety of Build Systems that are at our disposal and we no longer need to rely on such poor practices. Mobile.BuildTools can help turn your run of the mill project into a streamlined DevOps masterpiece. Best of all because the Mobile.BuildTools simply provide new targets for MSBuild it works absolutely EVERYWHERE that MSBuild itself is installed!

#### Background

As part of my frustration at how challenging it was to go from File -> New Solution to a base project that was ready to put into a DevOps pipeline, I set out to create the Prism QuickStart Templates. Part of the templates included many of the features you see in the Mobile.BuildTools. As time went on I realized the need to decouple the tools from the template so as new features were added, or bugs fixed it could be more easily added.

## Support

If this project helped you reduce time to develop and made your app better, please be sure to star the project & help support Dan.

[![GitHub Sponsors](https://github.blog/wp-content/uploads/2019/05/mona-heart-featured.png?fit=600%2C315)](https://xam.dev/sponsor-buildtools)


## Samples

- [App Config Demo](samples/AppConfigSample) - Sample Xamarin app using the new app.config. This sample uses Xamarin.Forms with Prism to show how you can use this with Dependency Injection and keep your apps testable with the IConfigurationManager rather than using all statics. NOTE: This project does not take advantage of build time transformations.

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
