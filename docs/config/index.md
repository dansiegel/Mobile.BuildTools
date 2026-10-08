# Choose a configuration API

Mobile.BuildTools offers two separate configuration approaches. Choose based on when values should change and how your code should read them.

| | AppSettings | XML ConfigurationManager |
| --- | --- | --- |
| Inputs | `appSettings` definitions in `buildtools.json`; flat `appsettings.json` or build environment variables | XML `app.config` and `app.{Environment}.config` transforms |
| Result | Strongly typed generated C# members | String values through `IConfigurationManager` / `ConfigurationManager.AppSettings` |
| Changes | Rebuild the binary | Build transforms, or runtime transforms when the relevant files are bundled |
| Packages | Released v2: `Mobile.BuildTools`; forthcoming v3: matching AppSettings generator and build assets | `Mobile.BuildTools.Configuration` for runtime use; matching build tasks for transforms and bundling |
| Startup | No initialization for the basic static class | Initialize ConfigurationManager before reading values; register it yourself if using DI |

`Mobile.BuildTools.Configuration` does not turn `appsettings.json` into a generated class. AppSettings does not require the ConfigurationManager package or its initialization. Neither automatically imports `dotnet user-secrets` or provides an automatic ASP.NET `IConfiguration` binding for generated values.

Both approaches put configuration into the application. Values shipped in generated code or bundled configuration can be extracted by a user. Keep confidential service credentials on your backend.

- [Generate and consume AppSettings](appsettings/index.md)
- [Configure AppSettings](appsettings/configuration.md)
- [Use XML app.config](app.config/index.md)
- [Initialize XML ConfigurationManager](app.config/in-code.md)
