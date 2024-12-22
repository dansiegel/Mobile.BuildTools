# Mobile.BuildTools.AppSettings

The Mobile.BuildTools.AppSettings is a re-invented implementation of the Classic Mobile.BuildTools that was originally generated as part of the Mobile.BuildTools project. This new implementation is significantly more powerful with an improved ability to control properties in Cross Compiled projects like those found in .NET MAUI and Uno Platform projects. This new implementation also provides an improved ability to bring in proeprties based on Prefixes and allows fuzzy matching for Configurations.

## Configuration

Be sure to add a `buildtools.json` to your solution root directory.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "appSettings": {
    "YourProjectName": [
      {
        "className": "AppSettings",
        "properties": [
          {
            "name": "PropertyName",
            "type": "String",
          },
          {
            "name": "SomeOtherProperty",
            "type": "String",
            "default": "Hello World"
          }
        ]
      }
    ]
  },
  "environment": {
    "defaults": {
      "SomeOtherProperty": "Hello Default Value"
    },
    "configuration": {
      "Debug": {
        "SomeOtherProperty": "Hello Debug Value"
      },
      "Android": {
        "SomeOtherProperty": "Hello Android Value"
      },
      "iOS_Debug": {
        "SomeOtherProperty": "Hello iOS Debug Value"
      }
    }
  }
}
```

As shown you can provide defaults either on the property or in the environment configuration in your buildtools.json. Additionally you can provide values in an appsettings.json file which can be anywhere between your project and your solution.
```json
{
  "PropertyName": "Hello World"
}
```

You can also provide values in an appsettings.Debug.json file which can be anywhere between your project and your solution.
```json
{
  "PropertyName": "Hello Debug World"
}
```
