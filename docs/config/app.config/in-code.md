# Using It In Code

Note that all values from the AppSettings are strings by default. Any conversions will need to be handled in your code.

```cs
var foo = ConfigurationManager.AppSettings["Foo"];
```

## Initialization

Before using the ConfigurationManager you must initialize it.

AppDelegate.cs

```cs
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    ConfigurationManager.Init();
    global::Xamarin.Forms.Forms.Init();
    LoadApplication(new App());

    return base.FinishedLaunching(app, options);
}
```

MainActivity.cs

```cs
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

```cs
var foo = ConfigurationManager.AppSettings["foo"]; // My Foo
ConfigurationManager.TransformForEnvironment("foo");
foo = ConfigurationManager.AppSettings["foo"]; // Transformed Value
```

To convert back you can simply call:

```cs
ConfigurationManager.TransformDefault();
```