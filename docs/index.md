# Getting Started

The Mobile.BuildTools is an easy to use NuGet package that adds new MSBuild targets to your build pipeline. In essenence it teaches MSBuild some new tricks to help make your DevOps easier and help you follow better practices while developing your application. The bulk of support is around Xamarin.Android and Xamarin.iOS and it will work regardless of whether you are using the native tooling, Xamarin.Forms, or Uno to create your UI.

The Mobile.BuildTools are a collection of MSBuild Tasks that help make MSBuild smarter in handling the build process for CI/CD with Mobile Applications. The library was born from a desire to share build processes from one app to the next without having to copy and paste a bunch of build scripts each of which could easily end up out of date. Because the Mobile.BuildTools simply provides MSBuild Tasks, this adds nothing to the size of your application and if being used on a project that will be packed and shared, you can set the PackageReference's PrivateAssets to all.

## Support

This project is maintained by Dan Siegel. If this project has helped you please consider sponsoring Dan on GitHub. Your contributions help make great open source projects possible.

[![GitHub Sponsors](https://github.blog/wp-content/uploads/2019/05/mona-heart-featured.png?fit=600%2C315)][sponsor]

## Latest NuGet's

| Package | NuGet | MyGet |
| --------------- | ----- | ---- |
| [Mobile.BuildTools][BuildToolsNuGet] | [![Latest NuGet][BuildToolsNuGetShield]][BuildToolsNuGet] | [![Latest CI Package][BuildToolsMyGetShield]][BuildToolsMyGet] |
| [Mobile.BuildTools.Configuration][BuildToolsConfigNuGet] | [![Latest NuGet][BuildToolsConfigNuGetShield]][BuildToolsConfigNuGet] | [![Latest CI Package][BuildToolsConfigMyGetShield]][BuildToolsConfigMyGet] |

Want to consume the CI packages? You can add this as a NuGet.config in your project root and Visual Studio will automatically pick up the configuration to provide packages from the CI Feed. Note that packages from this feed have passed all of the tests, but may have code that is still unstable.

```xml
<configuration>
  <packageSources>
    <clear />
    <add key="DanSiegel-MyGet" value="https://www.myget.org/F/dansiegel/api/v3/index.json" />
    <add key="NuGet.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

[sponsor]: https://xam.dev/sponor-buildtools

[PrismNuGetShield]: https://img.shields.io/nuget/vpre/Prism.MFractor.Config.svg

[BuildToolsNuGet]: https://www.nuget.org/packages/Mobile.BuildTools/
[BuildToolsNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.svg
[BuildToolsMyGet]: https://www.myget.org/feed/dansiegel/package/nuget/Mobile.BuildTools/
[BuildToolsMyGetShield]: https://img.shields.io/myget/dansiegel/vpre/Mobile.BuildTools.svg

[BuildToolsConfigNuGet]: https://www.nuget.org/packages/Mobile.BuildTools.Configuration/
[BuildToolsConfigNuGetShield]: https://img.shields.io/nuget/vpre/Mobile.BuildTools.Configuration.svg
[BuildToolsConfigMyGet]: https://www.myget.org/feed/dansiegel/package/nuget/Mobile.BuildTools.Configuration/
[BuildToolsConfigMyGetShield]: https://img.shields.io/myget/dansiegel/vpre/Mobile.BuildTools.Configuration.svg