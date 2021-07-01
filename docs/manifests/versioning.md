# Build Versioning

Build versioning can be extremely important for analytics and diagnostics. What's more is that Mobile development requires unique builds. No longer can you be lazy and ship apps for 15 years at Version 1.0.0.0. Ok technically all of your binaries in the application all will show that version, but the app itself must have a unique build number to allow you to upload to the App Store and Google Play.

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

## Planned Enhancements

Build Versioning is a brand new task that has been planned for a long time and sadly has taken a lot longer to get implemented than what was originally anticipated. Beginning with the push for 2.1 we will be looking at more advanced scenarios:

- Support scenarios where you may want to control a public display version like 1.0 but need a unique build id so that you can resubmit to the store if the App Store or Google Play reject your app during review.
- Support using GitVersioning. Git Versioning is a popular technique used by a lot of modern libraries including the Mobile.BuildTools. This occurs by evaluating the Git Height, and is generally controlled with a `version.json` in the root directory. You can look at the Mobile.BuildTools repo for an example of this using Nerdbank.GitVersioning.
