# App.config

The Mobile.BuildTools now includes support for using an app.config. It's important to note that we do not use the System.Configuration.ConfigurationManager, and instead use a lightweight custom implementation that allows you to initialize custom configurations at runtime which may not follow the typical app.config naming or perform transformations at runtime though this is generally not a good practice.

By default Mobile.BuildTools will look for any file in the root of the head project named app.config or app.*.config. All of those files will be bundled automatically into the native app. If your file has an environment config for the build configuration such as app.debug.config this will perform a transform during build on the bundled app.config.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "appConfig": {
    "strategy": "TransformOnly"
  }
}
```

!!! note
    By default the Mobile.BuildTools will only bundle the transformed app.config into your project. You can optionally set the strategy to `BundleAll` or `BundleNonStandard` if you require runtime transformations.

### App Config Strategy

| Strategy | Description |
|:--------:|-------------|
| TransformOnly | This the default strategy which will perform the transformation and only bundle a single transformed app.config into your project. |
| BundleAll | When set as the app config strategy this will bundle any app.config that you may have such as `app.debug.config` or `app.release.config` |
| BundleNonStandard | When set as the app config strategy this will limit bundled app config's to any that are not for standard Xamarin build configurations which include Debug, Release, Store, & AdHoc. |

!!! note
    All file names are compared ignoring case.

## Package and target versions

Install `Mobile.BuildTools.Configuration` in each project that uses its runtime API. Use matching build assets in the app project that performs XML transforms and bundling. Released v2 uses the `Mobile.BuildTools` tasks; current v3 source separates `Mobile.BuildTools.Configuration.MSBuild` and core tasks. Do not install this runtime package solely to generate AppSettings.

Released v2.0.245 has legacy platform assets. Current source targets `net10.0`, Android, iOS, macOS, Mac Catalyst, and Windows (when enabled). These target declarations describe forthcoming v3; they do not certify every SDK, runtime, or deployment scenario. See [modern .NET notes](../../maui.md) and validate the target you use.

Bundled configuration values can be extracted from the app. XML transformations do not provide a secure store for confidential service credentials.

## F.A.Q.

Q. Can I use the ConfigurationManager without using the Mobile.BuildTools?
A. Yes you absolutely can. Initialize the runtime package explicitly and supply its XML inputs in the locations expected by the target platform. See [Using it in code](in-code.md).

Q. How do I use the Environments?
A. By default Environments are disabled. This means that we will only copy the transformed app.config into your project and the ConfigurationManager will only read the primary app.config.