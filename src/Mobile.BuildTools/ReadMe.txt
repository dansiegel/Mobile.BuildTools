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

## Image & Icon Management

One of the great new features in Mobile.BuildTools 2.0 is the fact that we now natively handle your image assets. You can now safely remove all of those images that just clutter up your project and aren't great for Git in the first place. So how does it work? Let's assume you have a folder structure like the following in the root of your solution:

```
- Images
  - Shared
  - iOS
  - Android
  - Dev
  - Prod
```

Give this structure we might update our projects as follows:

```xml
<Project>
  <PropertyGroup>
    <ImageAssetsDirectory>..\..\Images\Shared;..\..\Images\iOS</ImageAssetsDirectory>
  </PropertyGroup>
  <PropertyGroup Condition=" $(Configuration) == 'Debug' >
    <ImageAssetsDirectory>$(ImageAssetsDirectory);..\..\Images\Dev</ImageAssetsDirectory>
  </PropertyGroup>
  <PropertyGroup Condition=" $(Configuration) == 'Store' >
    <ImageAssetsDirectory>$(ImageAssetsDirectory);..\..\Images\Prod</ImageAssetsDirectory>
  </PropertyGroup>
</Project>
```

Now when you build it will automatically pick up the images in any of the specified image folders.

### Images on iOS

Each platform is a little different and as such it will generate images based on how the platform works. When the Image targets run, it will scan your iOS project for App Icon Sets. The key here is that it is going to look for a matching output name and at the Contents.json for what actual outputs need to be created. If no icon set is found it will create them as more traditional Resources with a @1x, @2x, & @3x output. The @3x version will be the original size of the image in your Images folder.