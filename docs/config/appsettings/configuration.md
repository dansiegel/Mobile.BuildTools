# App Settings configuration

Start with the [complete walkthrough](index.md), then use this reference to customize generation. Both released v2 and forthcoming v3 source use `appSettings`; `projectSecrets` is an obsolete migration input.

## Select the compiling project

The keys under `appSettings` must exactly match `$(MSBuildProjectName)`, normally the `.csproj` filename without its extension, including case. Each value is an array of class definitions. The key is not an arbitrary alias, assembly name, root namespace, or generated class name.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "appSettings": {
    "AwesomeApp.Core": [
      {
        "accessibility": "Public",
        "rootNamespace": "Company.Product",
        "namespace": "Configuration",
        "className": "ApiSettings",
        "prefix": "Api_",
        "delimiter": ";",
        "properties": [
          { "name": "BaseUri", "type": "Uri", "defaultValue": "https://api.example.com/" },
          { "name": "Scopes", "type": "String", "isArray": true, "defaultValue": "read;profile" }
        ]
      }
    ]
  }
}
```

This selects `AwesomeApp.Core.csproj` and generates `Company.Product.Configuration.ApiSettings`. Its inputs are `Api_BaseUri` and `Api_Scopes`, with unprefixed names accepted as fallbacks. The array is generated as `string[]`, not a `List<string>`.

## Class options

| Field | Default | Purpose |
| --- | --- | --- |
| `accessibility` | `Internal` | Use `Public` if another assembly must access the generated class. |
| `rootNamespace` | Compiling project's `RootNamespace` | Base namespace; does not select the project. |
| `namespace` | `Helpers` | Relative namespace appended to the root. Set `"."` for the root namespace alone. |
| `className` | `AppSettings` | Generated class name. Set distinct names for multiple classes. |
| `prefix` | `BuildTools_` | Input lookup prefix; keep a trailing underscore for consistent v2/v3 behavior. |
| `delimiter` | `;` | Separator for array values. Use a single character for consistent behavior across versions. |
| `properties` | Empty | Explicit definitions of the members to generate. |

Input matching is case-insensitive, which accommodates build hosts that uppercase environment variable names. Project selection and file discovery are separate from that matching.

## Property options

Use `name`, `type`, optional `isArray`, and optional `defaultValue`. `array` and `default` are not the supported JSON property names.

Supported type names are `String`, `Bool`, `Byte`, `SByte`, `Char`, `Decimal`, `Double`, `Float`, `Int`, `UInt`, `Long`, `ULong`, `Short`, `UShort`, `DateTime`, `DateTimeOffset`, `Guid`, `Uri`, and `TimeSpan`. Supply values in a format valid for the selected type. Use a delimiter-separated string for an array.

`defaultValue` is a string, including for numeric and Boolean types. `"null"` and `"default"` request the type's default value; these are not the literal string values `"null"` and `"default"`. Prefer explicit safe defaults such as `"false"` for a Boolean or `"514"` for an integer. If no input or default exists, generation reports a missing value; v3 reports `MBT404`.

## Multiple classes with repeated member names

Give each class a unique name and prefix:

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "appSettings": {
    "AwesomeApp.Core": [
      {
        "className": "FooApiSettings",
        "prefix": "FooApi_",
        "properties": [{ "name": "BaseUri", "type": "Uri" }]
      },
      {
        "className": "BarApiSettings",
        "prefix": "BarApi_",
        "properties": [{ "name": "BaseUri", "type": "Uri" }]
      }
    ]
  }
}
```

Supply a flat `appsettings.json`:

```json
{
  "FooApi_BaseUri": "https://foo.example.com/",
  "BarApi_BaseUri": "https://bar.example.com/"
}
```

## Defaults and build configurations

`environment.defaults` and `environment.configuration` provide build inputs without separate local files. Keep only non-confidential defaults in this checked-in file:

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "environment": {
    "defaults": {
      "BuildTools_BackendUri": "https://api.example.com/"
    },
    "configuration": {
      "Debug": {
        "BuildTools_BackendUri": "https://dev.example.com/"
      },
      "Release": {
        "BuildTools_BackendUri": "https://api.example.com/"
      }
    }
  }
}
```

These inputs still need corresponding `appSettings` property definitions. System environment and JSON values can replace the defaults. See [lookup and precedence](index.md#file-lookup-and-precedence) for the important v2/v3 differences.

Forthcoming v3 source also supports platform and `Platform_Configuration` entries, such as `Android` and `iOS_Debug`. Its optional `environment.enableFuzzyMatching` matches some configuration prefixes when an exact match is absent. Prefer exact configuration names for predictable builds; this behavior is not part of released v2.0.245.
