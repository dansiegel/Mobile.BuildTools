<p align="center"><img src="logo/horizontal.svg" alt="Mobile.BuildTools" height="150px"></p>

# Build Tools

There is a lot of talk these days about DevOps. One of the problems with DevOps is that it can be really challenging. Far too many companies suffer from reliance on poor practices that their Development teams know need to be fixed. Today we have a variety of Build Systems that are at our disposal and we no longer need to rely on such poor practices. Mobile.BuildTools can help turn your run of the mill project into a streamlined DevOps masterpiece. Best of all because the Mobile.BuildTools simply provide new targets for MSBuild it works absolutely EVERYWHERE that MSBuild itself is installed!

> **IMPORTANT** Version 1.x is no longer under development. This is considered a legacy version. Docs for v1.x are located in the Wiki. For those wishing to use the more powerful API's described in the [mobilebuildtools.com docs](https://mobilebuildtools.com) site please use the v2.0 previews.

#### Background

As part of my frustration at how challenging it was to go from File -> New Solution to a base project that was ready to put into a DevOps pipeline, I set out to create the Prism QuickStart Templates. Part of the templates included many of the features you see in the Mobile.BuildTools. As time went on I realized the need to decouple the tools from the template so as new features were added, or bugs fixed it could be more easily added.

## Support

If this project helped you reduce time to develop and made your app better, please be sure to star the project.

Enterprise Support options are available through AvantiPoint - Email dsiegel@avantipoint.com

## Modern .NET applications

For new mobile applications, use .NET MAUI or Uno Platform. Start with the [Mobile.BuildTools documentation](https://mobilebuildtools.com) and the [.NET MAUI and Uno Platform notes](https://mobilebuildtools.com/maui/). Platform-specific tasks depend on your target SDK and package version.

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
