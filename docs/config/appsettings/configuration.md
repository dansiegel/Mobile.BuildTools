Configuration for App Settings is a little different than for Secrets in Mobile.BuildTools 1.x. While we do provide legacy support for getting values from the secrets.json this has been almost entirely rewritten to provide much more advanced scenarios as well as to solve some issues faced in CI.

## CI Platform Issues

A common problem with some build platforms such as Azure DevOps Windows Agents is that they cast all variables with ToUpper. This means that if you added `MyVariable`, what the Mobile.BuildTools would actually find was `MYVARIABLE`. With 1.x we had no way to convert this back to `MyVariable` for the source generation. This naturally caused a lot of problems. To solve this issue the Mobile.BuildTools now provides a configuration file that describes the projects and classes that it should generate.

## Configuring the buildtools.json

The Mobile.BuildTools 2.0 configuration gives us a lot of flexibility as we can define 1 or MORE classes that should be generated automatically at build. Additionally since we can describe the class in json, we can specify exactly the data type should be rather than relying on the Mobile.BuildTools to make an educated guess.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "appSettings": {
    "AwesomeApp": [
      {
        // Your configuration here
      }
    ]
  }
}
```

### Class Configuration

Within the Project we can now provide any configuration values we need to either override or explicitly provide that will control how the class will be generated.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "projectSecrets": {
    "AwesomeApp": [
      {
        "accessibility": "Internal",
        "className": "AppSettings",
        "delimiter": ";",
        "namespace": "Helpers",
        "rootNamespace": null,
        "properties": [
          // Property Definitions
        ]
      }
    ]
  }
}
```

!!! note
    If we do not provide any of the values shown above they will automatically default as shown. Only the `properties` are required.

### Property Configuration

The Mobile.BuildTools 2.0 added support for every primitive datatype, along with DateTime, DateTimeOffset, Uri, & Guid. Additionally you now have the ability to easily specify that a value should be an array. Note that this only generates arrays, you cannot specify other types such as `List` or `Collection`.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "projectSecrets": {
    "AwesomeApp": [
      {
        "properties": [
          {
            "name": "MyProperty",
            "type": "String"
          },
          {
            "name": "MyProperty2",
            "type": "Int"
          },
          {
            "name": "MyProperty3",
            "type": "String",
            "array": true
          }
        ]
      }
    ]
  }
}
```

### Providing a Default Value

There are a lot of values which either may not be sensitive or a default value would be safe to have in source control. We'll look at two scenarios here.

#### Default value for App Center App Id

It's pretty common for people using the Mobile.BuildTools to inject values such as the App Center App Id to be able to initialize the App Center SDK. It's also pretty common that you may not want to provide a value for local development. The Mobile.BuildTools makes this easy by understanding the reserved values of `null` and `default`. It doesn't matter which one you use, the Mobile.BuildTools will actually generate the code using the `default` keyword as this is safe across all data types, nullable, and non-nullable alike.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "appSettings": {
    "AwesomeApp": [
      {
        "properties": [
          {
            "name": "AppCenterAppId",
            "type": "String",
            "defaultValue": "null"
          }
        ]
      }
    ]
  }
}
```

#### User specified Default value

Other times we may have non-sensitive values that we need to configure defaults for. In the following scenario we may be setting up UDP logging with a Syslog Server. We know that the default port for UDP Syslog Servers is port 514. This value can simply be placed as 

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "appSettings": {
    "AwesomeApp": [
      {
        "properties": [
          {
            "name": "SyslogServerPort",
            "type": "Int",
            "defaultValue": "514"
          }
        ]
      }
    ]
  }
}
```
