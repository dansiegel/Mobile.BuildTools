# Using It In Code

This page uses XML `app.config` and the `Mobile.BuildTools.Configuration` runtime package. It does not consume the generated class or JSON inputs described by [AppSettings](../appsettings/index.md). ConfigurationManager values are strings; perform conversions in your application.

```csharp
using Mobile.BuildTools.Configuration;

ConfigurationManager.Init(enableRuntimeEnvironments: false);
var foo = ConfigurationManager.AppSettings["Foo"];
```

## Initialization

Before using the ConfigurationManager you must initialize it.

ConfigurationManager accepts a `bool` to enable runtime environments, this must be set to `true` if you want to use configuration transforms.

Call this during application startup, before reading configuration values:

```csharp
ConfigurationManager.Init(enableRuntimeEnvironments: true);
```

This initializes Mobile.BuildTools.Configuration; UI framework initialization is handled separately by your application.

## Transformations

While the Mobile.BuildTools will automatically perform transformations at Build, runtime transformations are also supported for those scenarios where you may need to change environments for whatever business reason.

For this let's consider that we have `app.config` and `app.foo.config`. We can transform to the Foo environment as follows:

```csharp
var foo = ConfigurationManager.AppSettings["foo"]; // My Foo
ConfigurationManager.Transform("foo"); // This is not case sensitive
foo = ConfigurationManager.AppSettings["foo"]; // Transformed Value
```

To convert back you can simply call:

```csharp
ConfigurationManager.Reset();
```

!!! note Note
    In order to Transform the values in the ConfigurationManager at Runtime the ConfigurationManager must be initialized with the `enableRuntimeEnvironments` parameter set to true. `ConfigurationManager.Init(true)`

!!! note Note
    Calling Transform for an Environment that does not exist will not throw an error, it will however call Reset to restore the ConfigurationManager to it's original state.

## Testability

The ConfigurationManager is Interface based and utilizes a Singleton. The singleton remains constant as long as ConfigurationManager.Init() is not called. You can Reset or Transform as often as you need. As a best practice it is recommended that you register the ConfigurationManager.Current instance with a Dependency Injection container and inject the IConfigurationManager into your code. This will allow you to mock the ConfigurationManager and better test your code.

## Dependency injection

Initialization and registration are explicit for this runtime API. If your app uses `Microsoft.Extensions.DependencyInjection`, register the initialized manager during startup:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Mobile.BuildTools.Configuration;

// In your application's startup code, with an IServiceCollection named services:
var manager = ConfigurationManager.Init(enableRuntimeEnvironments: true);
services.AddSingleton<IConfigurationManager>(manager);
```

Consumers can inject `IConfigurationManager` and read `manager.AppSettings["Foo"]`. Enable runtime environments only when the required transform files are bundled; see [app.config strategies](index.md#app-config-strategy). Forthcoming v3 also includes `AddBuildToolsConfiguration` extensions for `IHostBuilder` and `IConfigurationBuilder`; those source APIs are not a claim about released v2.0.245. Generated AppSettings members require none of this initialization.
