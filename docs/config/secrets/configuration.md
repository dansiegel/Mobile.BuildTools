Configuration for Secrets is a little bit different and required for v2. This provides a few benefits.

- It solves the issue of case sensitivity in which some build agents like to run `ToUpper()` on all variable names making it utterly impossible to properly generate the secrets.json properly without a script.
- It makes things just a little less magical so that other developers can more quickly generate the secrets.json in the appropriate projects to get started.
- It makes it possible to support more property types as v2.X now supports all primitives, Uri, Guid, TimeSpan, DateTime, & DateTimeOffset, plus the option to use an array for any property type.

We can configure the secrets for all of the projects in our solution by providing a value with the Project name and the appropriate configuration in the `buildtools.json` which should be located in the same directory as our solution file.

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
        {
          "name": "SampleString",
          "type": "String",
          "isArray": false
        },
        {
          "name": "SampleBool",
          "type": "bool",
          "isArray": false
        },
        {
          "name": "SampleInt",
          "type": "int",
          "isArray": false
        },
        {
          "name": "SampleDouble",
          "type": "double",
          "isArray": false
        },
        {
          "name": "SampleUri",
          "type": "Uri",
          "isArray": false
        },
        {
          "name": "SampleDateTime",
          "type": "DateTime",
          "isArray": false
        },
      ]
    }
  }
```

!!! note Note
    The configuration shown here is verbose. As long as the Properties are specified with a `name` and `type` the rest of the values shown here will default to the values shown.

!!! info Info
    If you wish to entirely disable the build task from even running in the MSBuild pipeline for a particular project you should specify the project name and set `disable` to `true`.