# App Settings

AppSettings generates strongly typed C# values at build time from `buildtools.json`, environment variables, and `appsettings.json`. Use the generated class directly; changing a value requires rebuilding your application. These values are compiled into your app and can be recovered from a shipped binary. Use this for environment configuration, not for confidential server credentials.

## Choose your package version

| Version | Generation | Package in the project receiving the class |
| --- | --- | --- |
| Released **2.0.245** | MSBuild writes a generated `.g.cs` file under `obj` and adds it to compilation | `Mobile.BuildTools` 2.0.245 |
| Forthcoming **v3**, described by the current source | Roslyn source generator with an MSBuild environment preparation step | `Mobile.BuildTools.AppSettings` with its matching `Mobile.BuildTools.Core` dependency/build assets |

The v3 notes describe source behavior, not a v3 release announcement. A v2 package does not acquire v3 behavior by copying a newer configuration example. The schema URL currently used by the source remains `https://mobilebuildtools.com/schemas/v2/buildtools.schema.json`.

`Mobile.BuildTools.Configuration` is the separate XML `app.config` runtime API. It is unnecessary for generated AppSettings and does not read `appsettings.json`. Neither API automatically imports `dotnet user-secrets`. See [choosing a configuration API](../index.md).

## Install, configure, build, and consume

This example generates settings in an existing class library named `AwesomeApp.Core.csproj`, which a .NET MAUI or Uno Platform app can reference. You can instead generate directly in your app project: use its exact project filename without `.csproj` as the key. A shared source project (`.shproj`) is compiled by its host projects; configure the compiling `.csproj` projects rather than treating it as an independent assembly.

### 1. Install in the project that receives the class

For released v2:

```sh
dotnet add src/AwesomeApp.Core/AwesomeApp.Core.csproj package Mobile.BuildTools --version 2.0.245
```

Set `PrivateAssets="all"` on the package reference to keep this build dependency from flowing to consumers of your library:

```xml
<PackageReference Include="Mobile.BuildTools" Version="2.0.245" PrivateAssets="all" />
```

This pins the published v2 implementation; it does not assert compatibility with every current .NET SDK. For new MAUI/Uno apps, check the [modern .NET notes](../../maui.md) and use a package compatible with your SDK. If testing a forthcoming v3 package, install `Mobile.BuildTools.AppSettings` into this same compiling project. Keep its matching `Mobile.BuildTools.Core` dependency and build assets; do not exclude its analyzer assets. Use the exact package versions and source supplied by the preview distribution rather than mixing v2/v3 packages. This guide does not name an unverified public v3 feed or version.

### 2. Put the configuration next to the solution

Use this layout in your existing solution:

```text
AwesomeApp/
  AwesomeApp.sln
  buildtools.json
  appsettings.json
  src/
    AwesomeApp.Core/
      AwesomeApp.Core.csproj
      Example.cs
    AwesomeApp/
      AwesomeApp.csproj
```

Create `buildtools.json` explicitly. Older tooling may create it on the first build, but the file is on disk and may not appear in Solution Explorer. Creating it inside a project does not replace the solution-level configuration.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "appSettings": {
    "AwesomeApp.Core": [
      {
        "accessibility": "Public",
        "rootNamespace": "AwesomeApp.Core",
        "namespace": "Helpers",
        "className": "AppSettings",
        "prefix": "BuildTools_",
        "properties": [
          { "name": "BackendUri", "type": "Uri" },
          { "name": "DisplayName", "type": "String", "defaultValue": "Local development" },
          { "name": "EnableDiagnostics", "type": "Bool", "defaultValue": "false" }
        ]
      }
    ]
  }
}
```

`AwesomeApp.Core` selects **AwesomeApp.Core.csproj**, independently of its namespace. `rootNamespace`, `namespace`, and `className` produce `AwesomeApp.Core.Helpers.AppSettings`. `Public` lets another assembly access the class; the default is `Internal`.

### 3. Supply safe local values

At the solution root, create `appsettings.json` as a flat key/value dictionary:

```json
{
  "BuildTools_BackendUri": "https://api.example.com/",
  "BuildTools_DisplayName": "Local development"
}
```

The prefix identifies an input key, not part of the generated member name. `BuildTools_BackendUri` becomes `AppSettings.BackendUri`. An unprefixed `BackendUri` is also accepted as a fallback. Do not use ASP.NET-style nested sections for these examples.

For configuration-specific values in the same directory, use `appsettings.Debug.json`, `appsettings.Release.json`, or your exact configuration name. See the version-specific lookup rules below before placing additional files in project directories.

Keep `buildtools.json` with safe defaults in source control. If local files contain private environment values, ignore them and commit a sanitized example file instead:

```gitignore
**/appsettings.json
**/appsettings.*.json
!**/appsettings.example.json
**/obj/
**/bin/
```

Ignoring a file does not remove it from Git if it is already tracked. Values in generated code, intermediate environment files, build logs, and binaries also need appropriate handling.

### 4. Build and use the generated class

Build the solution so the solution directory is unambiguous:

```sh
dotnet build AwesomeApp.sln --configuration Debug
```

In `src/AwesomeApp.Core/Example.cs`:

```csharp
using System;
using AwesomeApp.Core.Helpers;

namespace AwesomeApp.Core
{
    public static class Example
    {
        public static Uri Backend => AppSettings.BackendUri;
        public static string Name => AppSettings.DisplayName;
        public static bool Diagnostics => AppSettings.EnableDiagnostics;
    }
}
```

Released v2 can encounter an MSBuild target-ordering error where `SecretsJsonTask` reports that `ConfigurationPath` was not supplied. If that happens, explicitly pass the directory containing `buildtools.json`; from the solution root in a POSIX shell:

```sh
dotnet build AwesomeApp.sln --configuration Debug -p:BuildToolsConfigFilePath="$PWD"
```

This workaround and the generated C# example were verified with v2.0.245, a .NET Standard 2.0 library, and .NET SDK 6.0.421. It is not a compatibility claim for v2 on current MAUI SDKs. On another shell, pass the absolute solution directory using that shell's path syntax.

The generated class is part of the compilation. Do not copy it into your source tree or add it manually. This example needs no `ConfigurationManager.Init`, dependency injection registration, or `using Mobile.BuildTools.Configuration`. Reference `AwesomeApp.Core` from other projects to consume its public settings. See [troubleshooting](faq.md) if generation or editor completion is missing.

## File lookup and precedence

For the same input key, environment defaults are loaded first, then configuration-specific defaults, then system environment variables, then JSON files. Later loads replace earlier values. A property-level `defaultValue` is used only when no matching input key exists. A prefixed key is selected ahead of an unprefixed key, so avoid defining both forms for the same property.

### Released v2.0.245

V2 loads legacy `secrets.json` and `secrets.{Configuration}.json` for compatibility, then loads `appsettings.json` and `appsettings.{Configuration}.json` in **every directory from the project up to the solution**. At each directory, the configuration file follows the base file. Because the solution directory is visited last, its duplicate values can override project values. V2 does not use the v3 platform file selection described below.

### Forthcoming v3 source

V3 searches in this order: solution directory, project directory, existing `{Project}/{Platform}` and `{Project}/Platforms/{Platform}` directories, then project ancestors up to the solution. It removes duplicate directories and stops at the **first directory with an applicable settings file**. It does not merge solution and project settings files.

Within that selected directory, load order is:

1. `appsettings.json`
2. `appsettings.{Configuration}.json`
3. `appsettings.{Platform}.json`
4. `appsettings.{Platform}.{Configuration}.json`

For example, `appsettings.Android.Debug.json` follows `appsettings.Android.json`. Names use the platform enum spelling, such as `Android` and `iOS`; use exact filename casing on case-sensitive systems. A solution-level `appsettings.json` prevents lookup of a project-level override. Put related override files together in the selected directory.

V3 also loads legacy `secrets.json` files with a deprecation warning. Migrate to `appsettings.json`; do not rely on legacy file discovery for new configurations. Its environment configuration supports configuration, platform, and `Platform_Configuration` entries, in that order. JSON file names use dots; environment configuration keys use underscores.

These settings are also used by [manifest token replacement](../../manifests/index.md), which has additional `manifest.json` handling.
