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

| Package | NuGet | Sponsor Connect |
| --------------- | ----- | ----- |
| [Mobile.BuildTools][BuildToolsNuGet] | [![BuildToolsNuGetShield]][BuildToolsNuGet] | [![BuildToolsSponsorConnectShield]][BuildToolsSponsorConnect] |
| [Mobile.BuildTools.Configuration][BuildToolsConfigNuGet] | [![BuildToolsConfigNuGetShield]][BuildToolsConfigNuGet] | [![BuildToolsConfigSponsorConnectShield]][BuildToolsConfigSponsorConnect] |

| | Status |
|-|-|
| Build | [![Build Status][AzureDevOpsBuildStatus]][AzureDevOpsLatestBuild] |
| Integration Tests (Mac) | [![Build Status](https://dev.azure.com/dansiegel/Mobile.BuildTools/_apis/build/status/dansiegel.Mobile.BuildTools?branchName=master&stageName=Run%20Tests&jobName=Integration%20Tests%20(Mac))][AzureDevOpsLatestBuild] |
| Integration Tests (Windows) | [![Build Status](https://dev.azure.com/dansiegel/Mobile.BuildTools/_apis/build/status/dansiegel.Mobile.BuildTools?branchName=master&stageName=Run%20Tests&jobName=Integration%20Tests%20(Windows))][AzureDevOpsLatestBuild] |
| Tests | ![Tests](https://img.shields.io/azure-devops/tests/dansiegel/Mobile.BuildTools/40/master) |

Want to consume the CI packages? Be sure to [sign up as a GitHub sponsor](https://xam.dev/sponsor-buildtools) and get the pacakges from Sponsor Connect.


[BuildToolsNuGet]: https://www.nuget.org/packages/Mobile.BuildTools/
[BuildToolsNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.svg

[BuildToolsConfigNuGet]: https://www.nuget.org/packages/Mobile.BuildTools.Configuration/
[BuildToolsConfigNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.Configuration.svg

[BuildToolsSponsorConnect]: https://sponsorconnect.dev/package/Mobile.BuildTools
[BuildToolsSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FMobile.BuildTools%2Fvpre

[BuildToolsConfigSponsorConnect]: https://sponsorconnect.dev/package/Mobile.BuildTools.Configuration
[BuildToolsConfigSponsorConnectShield]: https://img.shields.io/endpoint?url=https%3A%2F%2Fsponsorconnect.dev%2Fshield%2FMobile.BuildTools.Configuration%2Fvpre

[AzureDevOpsBuildStatus]: https://dev.azure.com/dansiegel/Mobile.BuildTools/_apis/build/status/dansiegel.Mobile.BuildTools?branchName=master&stageName=Run%20Build
[AzureDevOpsLatestBuild]: https://dev.azure.com/dansiegel/Mobile.BuildTools/_build/latest?definitionId=40&branchName=master
