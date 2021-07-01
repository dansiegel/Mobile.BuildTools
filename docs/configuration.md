The Mobile.BuildTools relies a lot on JSON configurations because JSON is easy for most developers to work with.

| FileName | Schema Url |
|:--------:|:----------:|
| secrets.json | n/a - JSON Dictionary |
| buildtools.json | https://mobilebuildtools.com/schemas/v2/buildtools.schema.json |
| {imageName}.json | https://mobilebuildtools.com/schemas/v2/resourceDefinition.schema.json |

## Secrets.json

Everyone has a different opinion of how they would like to set things up. While the Mobile.BuildTools is opinionated in certain ways, we also try to make efforts to meet developers where their needs are giving you some flexibility in configuration. Within a CI environment, the Mobile.BuildTools relies on Environment Variables to map values you need in your app. However this is a bit of a pain to deal with for local app development. The Mobile.BuildTools has long relied on a `secrets.json` file containing the dictionary values of the various variables you need for your build. Version 2.0 has added a few benefits to this allowing you to now pick and choose which file directory you would like the secrets.json to live in. This can be any directory from the directory where your Solution file is located up to the Project directory. This can be particularly helpful when you may be using the Mobile.BuildTools to supply values across multiple projects or even where you may be replacing certain values in your AndroidManifest.xml or Info.plist.

## BuildTools.json

One of the biggest changes in the Mobile.BuildTools 2.0 is the introduction of the `buildtools.json`. Because we provide an easy to use json configuration with a Json Schema you have the ability to get intellisense in Visual Studio, Visual Studio Code, as well as many other editors that support Json Schemas. This makes it much easier for you to configure the Mobile.BuildTools rather than relying on MSBuild properties which can confuse many developers. 

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json"
}
```

!!! note
    Some features may still utilize MSBuild parameters which can be defined in your CI Build to customize behavior during a CI build. An example of this would be an override to the Image search paths which can be particularly useful for White Labeling apps.

## Image Configuration Json

The Images API for the Mobile.BuildTools is incredibly powerful and dynamic. One of the ways that we support powerful image creation is by incorporating a configuration file for each image. By convention the configuration file should have the same file name (minus the file extension) of the image resource. You can then customize the output image resource name. This can be done globally or be specific on a specific platform like iOS or Android. Additionally you can configure a single input image to have multiple outputs. An example of this scenario could be that you have a resource that will be used for the App Icon. On Android you may output both the standard image and the "Launcher" image which may have additional padding to look good as a round icon.
