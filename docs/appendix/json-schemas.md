The Mobile.BuildTools relies a lot on JSON configurations because JSON is easy for most developers to work with.

| FileName | Schema Url |
|:--------:|:----------:|
| secrets.json | n/a - JSON Dictionary **DEPRECATED** |
| appsettings.json | n/a - JSON Dictionary |
| buildtools.json | https://mobilebuildtools.com/schemas/v3/buildtools.schema.json |
| {imageName}.json | https://mobilebuildtools.com/schemas/v3/resourceDefinition.schema.json |

The v3 schemas are generated from the v3 configuration models. Until the v3 documentation release publishes these URLs, use the checked-in files under `docs/schemas/v3/` for validation. Existing v2 schema files remain available for older packages.
