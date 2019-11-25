# App.config

The Mobile.BuildTools now includes support for using an app.config. It's important to note that we do not use the System.Configuration.ConfigurationManager, and instead use a lightweight custom implementation that allows you to initialize custom configurations at runtime which may not follow the typical app.config naming or perform transformations at runtime though this is generally not a good practice.

By default Mobile.BuildTools will look for any file in the root of the head project named app.config or app.*.config. All of those files will be bundled automatically into the native app. If your file has an environment config for the build configuration such as app.debug.config this will perform a transform during build on the bundled app.config.