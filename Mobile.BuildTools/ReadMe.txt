Thanks for installing the Mobile.BuildTools library!

The Mobile.BuildTools are here to help you follow best practices when developing your mobile applications. If you've been around Mobile.BuildTools for a while A LOT has changed since version 1.0.

## Secrets

Dealing with secrets is a pain in Mobile Apps. Secrets introduce a way to handle strongly typed configurations into your code at compile time. To add build secrets to your application simply add a file named secrets.json to the root of your project. This will generate a Secrets class in the temporary output (obj folder) at build time in the Helpers namespace.

{
  "Foo": "Bar",
  "IsTrue": true,
  "AnInt": 1,
  "ADouble": 1.01
}

namespace YourRootNamespace.Helpers
{
    public static class Secrets
    {
        public const string Foo = "Bar";
        public const bool IsTrue = true;
        public const int AnInt = 1;
        public const double ADouble = 1.01;
    }
}

## Configuration Manager

The Configuration Manager is new in Mobile.BuildTools 2.0. This was introduced from an idea from @ChaseFlorell. This is of course inspired from the typical app.config you might see on a traditional desktop app where you can simply drop in a new app.config and change how your app behaves without recompiling your app for a whole new environment. The ConfigurationManager is compiled for Xamarin Android, Xamarin iOS, Xamarin Mac, UWP, and is of course available for netstandard2.0 and netcoreapp.

The Mobile.BuildTools includes a number of helpers here. When you have build specific configs such as app.debug.config or app.release.config it will automatically transform the app.config that is loaded into your app for the specific environment. It is key to understand that any app.config or app.{environment}.config you have in a platform head project will automatically be included as a native asset. This means you have the ability to bundle configs like:

- app.qa1.config
- app.qa2.config
- app.stage.config
- app.prod.config

While for most applications it would never make any sense to bundle configs like this, there are scenarios where this make sense to change the configuration at runtime. Bundling multiple configs within your app is fully supported and you are able to transform or swap out configurations entirely.

For more information and configuration settings be sure to check out the Wiki at:
https://github.com/dansiegel/Mobile.BuildTools/wiki