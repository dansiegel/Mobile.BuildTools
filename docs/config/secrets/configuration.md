Configuration for Secrets is a little bit different and required for v2. This provides a few benefits.

- It solves the issue of case sensitivity in which some build agents like to run `ToUpper()` on all variable names making it utterly impossible to properly generate the secrets.json properly without a script.
- It makes things just a little less magical so that other developers can more quickly generate the secrets.json in the appropriate projects to get started.
- It makes it possible to support more property types as v2.X now supports all primitives, Uri, Guid, TimeSpan, DateTime, & DateTimeOffset, plus the option to use an array for any property type.

We can configure the secrets for all of the projects in our solution by providing a value with the Project name and the appropriate configuration in the `buildtools.json` which should be located in the same directory as our solution file. Note that the following configuration will enable the Secrets task to run for a project named `AwesomeApp` in your solution. All other projects in the solution will not run the Secrets tasks.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "projectSecrets": {
    "AwesomeApp": {
      // App Secret Configurations
    }
  }
}
```

## Project Configuration

Within the Project we can now provide any configuration values we need to either override or explicitly provide:

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "projectSecrets": {
    "AwesomeApp": {
      "delimiter": ";",
      "disable": false,
      "className": "Secrets",
      "namespace": "Helpers",
      "properties": [
        // Property Definitions
      ]
    }
  }
}
```

!!! note
    If we do not provide any of the values shown above they will automatically default as shown.

### Delimiter

The Delimiter is 

!!! note Note
    The configuration shown here is verbose. As long as the Properties are specified with a `name` and `type` the rest of the values shown here will default to the values shown.

!!! info Info
    If you wish to entirely disable the build task from even running in the MSBuild pipeline for a particular project you should specify the project name and set `disable` to `true`.

## Providing Default Values

There are some times in which some configuration values may in fact be perfectly safe to check into source control. Such examples could be a Retry Count. Each Property defined can additionally provide a default value as shown here:

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "projectSecrets": {
    "AwesomeApp": {
      "properties": [
        {
          "name": "RetryCount",
          "type": "Int",
          "defaultValue": "3"
        }
      ]
    }
  }
}
```

Other values may instead be best left with a null or default value. By convention the Mobile.BuildTools will generate all values as `default` regardless of the property type, except when the property type returns an array. For will always return an empty array. This behavior is consistent regardless of whether you mark the `defaultValue` as `null` or `default`.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "projectSecrets": {
    "AwesomeApp": {
      "properties": [
        {
          "name": "AppCenterToken",
          "type": "String",
          "defaultValue": "null"
        }
      ]
    }
  }
}
```