# Mobile.BuildTools.AppSettings

This package contains the Roslyn AppSettings generator used by the forthcoming v3 implementation. Its core MSBuild assets prepare the build environment. These source notes do not announce a v3 release or a public preview feed.

Install the matching `Mobile.BuildTools.AppSettings` package in the compiling project that should receive the generated class and retain its `Mobile.BuildTools.Core` dependency/build assets. `Mobile.BuildTools.Configuration` is a separate XML runtime API and is not needed for generated settings. Released v2.0.245 uses MSBuild generation through `Mobile.BuildTools` instead.

See the [complete AppSettings walkthrough](https://mobilebuildtools.com/config/appsettings/) for package placement, directory layout, build commands, and consumption.

## Configuration

Put `buildtools.json` beside your solution. The `AwesomeApp.Core` key selects `AwesomeApp.Core.csproj`, independently of its namespace:

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "appSettings": {
    "AwesomeApp.Core": [
      {
        "rootNamespace": "AwesomeApp.Core",
        "namespace": "Helpers",
        "className": "AppSettings",
        "properties": [
          { "name": "DisplayName", "type": "String", "defaultValue": "Local development" },
          { "name": "BackendUri", "type": "Uri" }
        ]
      }
    ]
  }
}
```

Supply `appsettings.json` in the same directory:

```json
{
  "BuildTools_BackendUri": "https://api.example.com/"
}
```

Build, then use `AwesomeApp.Core.Helpers.AppSettings.BackendUri` in the compiling project. The class is `Internal` by default; choose `Public` if another assembly must access it. `defaultValue`, not `default`, supplies an optional property's fallback. Array definitions use `isArray`.

V3 looks in the solution directory first and stops at the first directory with applicable JSON files, loading base, configuration, platform, and platform.configuration files there. It does not merge project-level overrides with solution-level files. [Lookup and precedence](https://mobilebuildtools.com/config/appsettings/#file-lookup-and-precedence) explains how this differs from released v2.

Generated values and intermediate environment files can expose configuration. Keep private files out of Git, and keep confidential server credentials out of shipped client apps. Neither generated AppSettings nor ConfigurationManager automatically reads `dotnet user-secrets`.
