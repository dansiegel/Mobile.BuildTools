# Continuous integration setup

Configure the class and properties in the checked-in solution-level `buildtools.json`, as shown in the [AppSettings walkthrough](../config/appsettings/index.md). Expose values as environment variables in the process that runs MSBuild. No separate secrets-generation command or local JSON file is required.

## Map inputs to generated members

| Definition | CI variable | Generated member |
| --- | --- | --- |
| `prefix: "BuildTools_"`, `name: "BackendUri"` | `BuildTools_BackendUri` | `AppSettings.BackendUri` |
| `prefix: "BuildTools_"`, `name: "DisplayName"` | `BuildTools_DisplayName` | `AppSettings.DisplayName` |
| `prefix: "Api_"`, `name: "BaseUri"` | `Api_BaseUri` | The configured class's `BaseUri` |

Input lookup is case-insensitive; `BUILDTOOLS_BACKENDURI` can match `BuildTools_BackendUri`. This does not make project names or JSON filenames case-insensitive. Do not use `Secret_`, `iOSSecret_`, or `DroidSecret_` as the default AppSettings mapping: those belong to older features.

For example, this build step uses safe public values after the checkout and SDK setup steps of your existing GitHub Actions workflow:

```yaml
- name: Build configured solution
  run: dotnet build AwesomeApp.sln --configuration Release
  env:
    BuildTools_BackendUri: https://api.example.com/
    BuildTools_DisplayName: Production
```

The same variables can be supplied by Azure Pipelines, AppVeyor, or another build host. Install the SDK/workloads required by your application and use an MBT version compatible with that SDK. Building a modern MAUI/Uno app does not make an older MBT binary compatible automatically.

## Control precedence

Environment variables override `environment.defaults` and configuration defaults. Later JSON loads can override the same environment keys. If CI should use only its environment values, omit local `appsettings.json` files from the checkout/build directory; commit a sanitized `appsettings.example.json` for developers instead.

See [v2/v3 file lookup and precedence](../config/appsettings/index.md#file-lookup-and-precedence). V2 merges files from project to solution; forthcoming v3 uses the first applicable directory, starting with the solution. Keep a property's prefix consistent between JSON and CI inputs.

Missing required inputs fail generation; v3 reports `MBT404`. Avoid a permissive default that hides a missing production setting.

## Configuration is compiled into the app

CI secret storage helps protect build inputs, but it does not keep values confidential after they are compiled into a client application. API client identifiers and public backend URLs can be appropriate inputs; confidential service credentials belong on a server.

Generated source, intermediate `buildtools.env` files, and diagnostic logs may contain values. Keep them out of published artifacts and redact them before sharing. Setting `debug: true` can expose additional information in build logs.
