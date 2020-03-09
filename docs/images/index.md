# Image Asset Generation

It takes about 5 minutes as a Mobile developer until you realize that Mobile apps have a lot of image assets. If you're a Cross Platform developer you realize that with your doubled reach comes a doubling of the amount of image assets you need to track. One of the premiere features of the Mobile.BuildTools in version 2 is the ability to handle custom asset generation. We do this by only tracking or letting you worry about the Full Resolution image assets that you want as part of your app.

!!! important
    It is extremely important to note that the Image Processing features listed here are largely WIP guidance on how these features are intended to work. As Image Processing is still very much WIP, not everything listed here may currently be functioning as expected.

## Adding Image Search Directories

The Mobile.BuildTools allows you to bring in images within one or more directories in 3 different ways:

- Directories that should be searched for all builds
```json
{
  "images": {
    "directories": [
      "Images"
    ]
  }
}
```

- Directories that should conditionally be searched. See [Conditional Directories](#conditional-directories) for more information.
```json
{
  "images": {
    "conditionalDirectories": {
      "someCondition": [ "AnotherDirectory" ]
    }
  }
}
```

- MSBuild Configured Directories typically used in CI which can optionally override your json configuration. See the [Build Customization](build-customization.md) topic for more information.

!!! warning
    The Mobile.BuildTools will only evaluate images in the top level of the specified directories. Images in subdirectories will be ignored unless otherwise specified as a search directory or conditional directory.

## Conditional Directories

Conditional Directories supercharges your ability deliver customized images for your apps. The Conditional Directories help us to get a better handle on what image assets we want to include either for a specified Target Platform or Build Configuration.

### Supported Conditions

**Platform Conditions**

- Xamarin.iOS
- MonoAndroid

**Build Configurations**

- Any Build Configuration you have!
- Optionally negate a Condition like `!Debug`

```json
{
  "images": {
    "directories": [
      "Images",
      "Images\\Shared"
    ],
    "conditionalDirectories": {
      "MonoAndroid": [ "Images\\Android" ],
      "Xamarin.iOS": [ "Images\\iOS" ],
      "Debug": [ "Images\\Debug" ],
      "!Store": [ "Images\\NotProduction" ],
      "Store": [ "Images\\Production" ]
    }
  }
}
```

### Sample Outputs

Using Conditional Directories along with the ability to watermark images (see [Configuring Images](configuring-images.md) topic) you can easily transform images.

For this let's say that we have two watermark images that we want to use on one or more of the images that will be used in our app.

| | |
|-|-|
| ![Dev Badge](/assets/samples/beta-version.png "beta version") | ![Example](/assets/samples/example.png "Example") |

Let's next say that we have two images we want to test this on like the following:

| | |
|-|-|
| ![.NET Bot](/assets/samples/dotnetbot.png "dotnetbot") | ![Mobile.BuildTools](/assets/samples/icon.png "Mobile.BuildTools") |

| | | |
|-|-|-|
| ![.NET Bot](/assets/samples/dotnetbot.png "dotnetbot") | ![.NET Bot - Dev](/assets/samples/dotnetbot-beta.png "dotnetbot - dev") | ![.NET Bot - Example](/assets/samples/dotnetbot-example.png "dotnetbot - Example") |

| | | |
|-|-|-|
| ![Mobile.BuildTools](/assets/samples/icon.png "Mobile.BuildTools") | ![Mobile.BuildTools - Dev](/assets/samples/icon-beta.png "Mobile.BuildTools - Dev") | ![Mobile.BuildTools - Example](/assets/samples/icon-example.png "Mobile.BuildTools - Example") |

!!! important
    You may have noticed from looking at these images that all of the images are different resolutions. The Mobile.BuildTools is smart enough to understand that we want to scale your watermark and your input image to share the same canvas size. We will then generate the appropriate output size based on what your needs are.
