# Configuration files

Put `buildtools.json` beside the solution file. It defines which projects receive generated AppSettings classes and configures other build tasks. Values for generated members come from flat JSON files, environment defaults, or the build process's environment.

| File | Purpose | Schema |
| --- | --- | --- |
| `buildtools.json` | Task and generated-class definitions | [BuildTools schema](schemas/v2/buildtools.schema.json) |
| `appsettings.json` | Flat dictionary of build-time values | No fixed schema; keys match your definitions |
| `appsettings.{Configuration}.json` | Configuration overrides | No fixed schema |
| `{imageName}.json` | Image resource configuration | [Resource definition schema](schemas/v2/resourceDefinition.schema.json) |

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json"
}
```

A schema reference alone does not define an AppSettings class. Add the project/class/property definitions from the [walkthrough](config/appsettings/index.md).

## Where settings files are loaded

Released v2.0.245 merges `appsettings.json` and configuration variants while walking from project to solution. Forthcoming v3 starts with the solution and stops at the first directory containing applicable settings files. See [lookup and precedence](config/appsettings/index.md#file-lookup-and-precedence) before relying on project overrides.

`secrets.json` and `projectSecrets` are obsolete compatibility inputs. Use `appsettings.json` and `appSettings` for current examples. `dotnet user-secrets` is not loaded automatically. The source continues to use the `/schemas/v2/` schema URL; no v3 schema URL is published here.

## Image configuration

An image's JSON file has the same base filename as the input image. It can describe output names, scales, watermarks, and platform-specific outputs. See [Image assets](images/index.md) and [Configuring images](images/configuring-images.md) for examples and feature limitations.
