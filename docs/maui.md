# .NET MAUI and Uno Platform

Use .NET MAUI or Uno Platform for new mobile applications. Mobile.BuildTools provides MSBuild tasks; configuration generation can be used in C# projects independently of the UI framework.

For application configuration, see [App Settings](config/appsettings/index.md). For XML configuration and runtime transformations, see [App.config](config/app.config/index.md).

Manifest and image processing depend on the target platform SDK, project layout, and Mobile.BuildTools version. Check the documentation for each task and validate it against your target project. Retained platform-specific code does not imply that every target or SDK is actively supported.

The [Microsoft Xamarin support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/xamarin) records the end of Xamarin support on May 1, 2024. Older Xamarin samples and platform notes reflect the project's origins; use modern .NET projects for new applications.

If you encounter an issue, [open a GitHub issue](https://github.com/dansiegel/Mobile.BuildTools/issues) with your Mobile.BuildTools version, target framework, SDK version, and a minimal reproduction.
