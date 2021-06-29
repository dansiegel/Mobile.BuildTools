Thanks for installing the Mobile.BuildTools library!

The Mobile.BuildTools are here to help you follow best practices when developing your mobile applications. If you've been around Mobile.BuildTools for a while A LOT has changed since version 1.0. Please be sure to check out the new docs site at https://mobilebuildtools.com

If the Mobile.BuildTools is helping you deliver your app please consider becoming a GitHub Sponsor.
https://github.com/sponsors/dansiegel

Be sure to subscribe to my YouTube channel for tips and tricks
https://youtube.com/dansiegel

## App Wide Config

Be sure to add a `buildtools.json` to your solution root directory.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "artifactCopy": {
    "disable": false // false by default
  },
  "automaticVersioning": {
    "behavior": "PreferBuildNumber|Timestamp|Off", // PreferBuildNumber by default
    "environment": "All|BuildHost|Local", // All by default
    "versionOffset": 100 // 0 by default
  },
  "css": {
    "bundleScss": false // false by default
  },
  "images": {
    // Note: All directories are relative from the configuration file
    "directories": [
      "Images/Shared"
    ],
    // Conditional Directory are added to the directories above
    // These can be either Build Configurations or Target Frameworks like iOS/Android
    "conditionalDirectories": {
      "Debug": [
        "Images/Debug"
      ],
      "Store": [
        "Images/Store"
      ],
      "Android": [
        "Images/Android"
      ]
    }
  },
  "projectSecrets": {
    "Contoso": {
      "properties": [
        {
          "name": "AzureAdClientId",
          "type": "String"
        },
        {
          "name": "AzureAdScopes",
          "type": "String",
          "isArray": true
        },
        {
          "name": "Backend",
          "type": "Uri"
        }
      ]
    }
  },
  "environment": {
    "Debug": {
      "AppId": "com.company.awesomeappdev",
      "Backend": "https://dev.api.awesomeapp.com"
    },
    "Release": {
      "AppId": "com.company.awesomeapp",
      "Backend": "https://api.awesomeapp.com"
    }
  }
}
```
