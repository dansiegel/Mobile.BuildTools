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
        "prefix": "BuildTools_",
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

#### Handling Duplicate Property Names

There may be times in which you have more than one project in your solution, or perhaps just more than one generated settings class that require duplicate property names. An example of this could be an API Settings class. 

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "appSettings": {
    "AwesomeApp": [
      {
        "className": "FooApiSettings",
        "properties": [
          {
            "name": "BaseUri",
            "type": "Uri"
          }
        ]
      },
      {
        "className": "BarApiSettings",
        "properties": [
          {
            "name": "BaseUri",
            "type": "Uri"
          }
        ]
      }
    ]
  }
}
```

In this sample we have 2 generated classes with the `BaseUri` property, this creates a few problems for us because we need to distinguish which Uri belongs to which class and ultimately our JSON would be invalid because it has a duplicated key

```json
{
  "BaseUri": "https://api.foo.com",
  "BaseUri": "https://api.bar.com"
}
```

To solve this problem we can use the Prefix property on our generated class settings. In this way we can specify a unique variable prefix that will be used to identify which Base Uri property belongs to which generated class

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "appSettings": {
    "AwesomeApp": [
      {
        "className": "FooApiSettings",
        "prefix": "FooApi_",
        "properties": [
          {
            "name": "BaseUri",
            "type": "Uri"
          }
        ]
      },
      {
        "className": "BarApiSettings",
        "prefix": "BarApi_",
        "properties": [
          {
            "name": "BaseUri",
            "type": "Uri"
          }
        ]
      }
    ]
  }
}
```

With the Prefix added we can now update our appsettings.json to be:

```json
{
  "FooApi_BaseUri": "https://api.foo.com",
  "BarApi_BaseUri": "https://api.bar.com"
}
```

> [!NOTE]
> If your prefix does not end with an underscore one will automatically be inserted. In the above example if we did not explicitly have the underscore, the Mobile.BuildTools would still be expecting the same values in our `appsettings.json` or as an Environment variable.

#### Setting Variables from the environment

The Mobile.BuildTools allows us to "Fake" environment variables. There may be times such as the previous sample with our previous example where the values aren't particularly sensitive but simply something that may change based on our Build... 

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "environment": {
    "defaults": {
      "FooApi_BaseUri": "https://dev.api.foo.com",
      "BarApi_BaseUri": "https://dev.api.bar.com"
    },
  }
}
```

It's also possible that we may want to further customize this without the need to update a CI Build environment for variables that aren't particularly sensitive. In this case we can provide Build Configuration specific settings:

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "environment": {
    "configuration": {
      "Debug": {
        "FooApi_BaseUri": "https://dev.api.foo.com",
        "BarApi_BaseUri": "https://dev.api.bar.com"
      },
      "QA": {
        "FooApi_BaseUri": "https://qa.api.foo.com",
        "BarApi_BaseUri": "https://qa.api.bar.com"
      },
      "Release": {
        "FooApi_BaseUri": "https://api.foo.com",
        "BarApi_BaseUri": "https://api.bar.com"
      }
    },
  }
}
```

#### Fuzzy Matching

From time to time you may want to make use of Fuzzy Matching. Fuzzy Matching allows you to provide configurations that aren't tied to a specific Build Configuration name. For example we might have an `appsettings.QA.json` with a `DebugQA` build configuration. We might also have a `ReleaseQA` build configuration. In the case we build with either of these build configurations we might want it to pick up the QA build configuration. We can pick these configurations up by enabling Fuzzy Matching in our environment.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "environment": {
    "enableFuzzyMatching": true
  }
}
```
