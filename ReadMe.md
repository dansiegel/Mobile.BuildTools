<p align="center"><img src="logo/horizontal.svg" alt="Mobile.BuildTools" height="150px"></p>

# Build Tools

There is a lot of talk these days about DevOps. One of the problems with DevOps is that it can be really challenging. Far too many companies suffer from reliance on poor practices that their Development teams know need to be fixed. Today we have a variety of Build Systems that are at our disposal and we no longer need to rely on such poor practices. Mobile.BuildTools can help turn your run of the mill project into a streamlined DevOps masterpiece. Best of all because the Mobile.BuildTools simply provide new targets for MSBuild it integrates with MSBuild. Use a package compatible with your target framework, SDK, and configured tasks.

> **Versions:** NuGet 2.0.245 is the released v2 package. Current source is being prepared for v3; v3-specific documentation does not announce a release. Version 1.x is no longer maintained; its historical docs are in the [Wiki](https://github.com/dansiegel/Mobile.BuildTools/wiki).

#### Background

As part of my frustration at how challenging it was to go from File -> New Solution to a base project that was ready to put into a DevOps pipeline, I set out to create the Prism QuickStart Templates. Part of the templates included many of the features you see in the Mobile.BuildTools. As time went on I realized the need to decouple the tools from the template so as new features were added, or bugs fixed it could be more easily added.

## Support

If this project helped you reduce time to develop and made your app better, please be sure to star the project.

Enterprise Support options are available through AvantiPoint - Email dsiegel@avantipoint.com

## Modern .NET applications

For new mobile applications, use .NET MAUI or Uno Platform. Start with the [AppSettings walkthrough](https://mobilebuildtools.com/config/appsettings/) and the [.NET MAUI and Uno Platform notes](https://mobilebuildtools.com/maui/). Platform-specific tasks depend on your target SDK and package version.

## Mobile.BuildTools

For more information on build tasks and properties, see the [documentation](https://mobilebuildtools.com).

| Package | NuGet |
| --------------- | ----- |
| [Mobile.BuildTools][BuildToolsNuGet] | [![BuildToolsNuGetShield]][BuildToolsNuGet] |
| [Mobile.BuildTools.Configuration][BuildToolsConfigNuGet] | [![BuildToolsConfigNuGetShield]][BuildToolsConfigNuGet] |

[BuildToolsNuGet]: https://www.nuget.org/packages/Mobile.BuildTools/
[BuildToolsNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.svg

[BuildToolsConfigNuGet]: https://www.nuget.org/packages/Mobile.BuildTools.Configuration/
[BuildToolsConfigNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.Configuration.svg

[AzureDevOpsBuildStatus]: https://dev.azure.com/dansiegel/Mobile.BuildTools/_apis/build/status/dansiegel.Mobile.BuildTools?branchName=master&stageName=Run%20Build
[AzureDevOpsLatestBuild]: https://dev.azure.com/dansiegel/Mobile.BuildTools/_build/latest?definitionId=40&branchName=master
