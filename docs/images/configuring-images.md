The first time the Mobile.BuildTools encounters an image it will automatically generate a default image configuration file along side of the image. This file is what allows you to customize and further refine the image that you want. A configuration file may reside a conditional search directory. The Mobile.BuildTools will opt to use a configuration that is not in the same file directory as the image resource any time that a duplication is found.

!!!! warning Warning
    Keep in mind that we only ever support a scenario where you have a single configuration in a directory other than the original image. In the event that two or more configurations are found for the same image in directories other than the directory where the image is located the Mobile.BuildTools will throw an exception causing a Build Error.

## Configuring Images

The Schema for configuring images is rather simple by design. We allow you to specify a watermark file name, a name, a scale and an optional ignore. To start let's consider that we have an image named `Mobile-BuildTools.png`. We know that we cannot get away with this file name on all platforms so we want to rename the generated image.

```json
{
  "name": "mobile_buildtools"
}
```

The above sample would allow us to have a resource named `mobile_buildtools` when we refer to this from our Xamarin code.

## Watermarking Images

One of the great things that the Mobile.BuildTools supports is watermarking.