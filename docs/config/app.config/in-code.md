# Using It In Code

Note that all values from the AppSettings are strings by default. Any conversions will need to be handled in your code.

```csharp
var foo = ConfigurationManager.AppSettings["Foo"];
```

## Initialization

Before using the ConfigurationManager you must initialize it.

AppDelegate.cs

```csharp
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    ConfigurationManager.Init();
    global::Xamarin.Forms.Forms.Init();
    LoadApplication(new App());

    return base.FinishedLaunching(app, options);
}
```

MainActivity.cs

```csharp
protected override void OnCreate(Bundle bundle)
{
    TabLayoutResource = Resource.Layout.Tabbar;
    ToolbarResource = Resource.Layout.Toolbar;

    base.OnCreate(bundle);

    ConfigurationManager.Init(this);

    global::Xamarin.Forms.Forms.Init(this, bundle);
    LoadApplication(new App());
}
```

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