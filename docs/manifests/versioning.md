# Build Versioning

Build versioning can be extremely important for analytics and diagnostics. What's more is that Mobile development requires unique builds. No longer can you be lazy and ship apps for 15 years at Version 1.0.0.0. Ok technically all of your binaries in the application all will show that version, but the app itself must have a unique build number to allow you to upload to the App Store and Google Play.

!!! danger "Critical Note"
    While this was originally slated for v2.0, this will not be done until 2.1.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "automaticVersioning": {
    "behavior": "PreferBuildNumber",
    "environment": "All",
    "versionOffset": 0,
    "disable": false
  },
}
```

Automatic Build Versioning supports the following Versioning Environments:

| Environment | Description |
| ----------- | ----------- |
| All | Versioning will occur on every build. |
| BuildHost | Versioning will only occur if a \*Supported Build Host is detected. |
| Local | Versioning will only occur if a \*Supported Build Host is not detected. |

Automatic Build Versioning supports the following `Behavior`'s:

| Behavior | Description |
| -------- | ----------- |
| Off | Automatic Versioning is Disabled |
| PreferBuildNumber | When running on a \*Supported Build Host it will use the Build Number, otherwise it will use the current timestamp |
| Timestamp | Automatic Versioning will use the timestamp for the build |

\* Supported Build Hosts:

  - AppCenter
  - AppVeyor
  - Azure DevOps
  - Jenkins

!!! info Info
    You might use the `versionOffset` when your CI Build Number and Build Number in the App Store or Google Play are not in sync. As an example, when shipping multiple APKs with the same build number Google Play may take build 123 and make it 100123, 200123, 300123, & 400123 respectively for each of the 4 APK's you have provided. This would mean when switching to AAB that you might need to offset by 400000 in order to get your new AAB build to show up in Google Play.