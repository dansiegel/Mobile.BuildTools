# JSON schemas

| File | Schema |
| --- | --- |
| `buildtools.json` | [BuildTools](../schemas/v2/buildtools.schema.json) |
| `{imageName}.json` | [Image resource definition](../schemas/v2/resourceDefinition.schema.json) |
| `appsettings.json` | Flat dictionary; no fixed schema |

The current source still uses `https://mobilebuildtools.com/schemas/v2/buildtools.schema.json`, including forthcoming v3 configuration examples. The URL path is not the package version. Use [the AppSettings reference](../config/appsettings/configuration.md) for project keys and generated-class definitions, and [version-specific file lookup](../config/appsettings/index.md#file-lookup-and-precedence) for value resolution.

`secrets.json` is an obsolete compatibility input. It is not the .NET user-secrets store.
