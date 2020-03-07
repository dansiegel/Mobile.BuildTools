# Image Asset Generation

!!!! critical Critical
    It is extremely important to note that the Image Processing features listed here are largely WIP guidance on how these features are intended to work. As Image Processing is still very much WIP, not everything listed here may currently be functioning as expected.

```json
{
  "images": {
    "directories": [
      "Images\\Beta",
      "Images\\Shared"
    ],
    "conditionalDirectories": {
      "MonoAndroid": [ "Images\\Android" ],
      "Xamarin.iOS": [ "Images\\iOS" ],
      "Debug": [ "Images\\Debug" ],
      "!Store": [ "Images\\NotProduction" ],
      "Store": [ "Images\\Production" ]
    },
    "watermarkOpacity": null,
    "disable": false
  }
}
```

!!! info Info
    While you should generally configure directories in the `buildtools.json`, there may be times which you need to conditionally add additonal search directories for your images. For these times you can use the build property `BuildToolsImageSearchPath` to set any additional directories in a semi-colon separated list as shown here:

!!! note Note
    If you have an image name that contains white space such as `foo bar.png`, the Mobile.BuildTools will sanitize the file name replace any contiguous white space characters with a single `-`. This normalization is required by Android and will mean that you will need to reference the image as `foo-bar.png`.