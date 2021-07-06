# App Manifests

There are many times in which you may need to parameterize an AndroidManifest.xml or Info.plist. One such example would be when using the MSAL library for Azure Active Directory / Azure Active Directory B2C user authentication in which you must create a custom url scheme like:

```xml
<key>CFBundleURLTypes</key>
<array>
  <dict>
    <key>CFBundleTypeRole</key>
    <string>Editor</string>
    <key>CFBundleURLName</key>
    <string>com.avantipoint.awesomeapp</string>
    <key>CFBundleURLSchemes</key>
    <array>
      <string>msal$AzureADClientId$</string>
    </array>
  </dict>
</array>
```

We can now leave our Info.plist or AndroidManifest.xml checked into source control and in place in our project. The Mobile.BuildTools will intelligently replace any tokenized values like the one above during the build.

```json
{
  "$schema": "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json",
  "manifests": {
    "token": "$$",
    "variablePrefix": "Manifest_",
    "missingTokensAsErrors": false,
    "disable": false
  },
```

!!! note Note
    If no environment variable can be found matching `Manifest_` as the prefix is defined, the Mobile.BuildTools will next search for a variable name matching the token name which would be `AzureADClientId` in the sample above. It will also try to do all matches case insensitive due to the issue with some build agents running `ToUpper()` on all variable names.

!!! info Info
    In order to work with the tokenized manifest locally without having to update your Environment Variables on your developer machine, you can simply drop in a `manifest.json` in the Project root with the Key/Value pairs for the Mobile.BuildTools to use. If using this file, be sure to add it to the .gitignore so as to not accidentally check it into source control.

## Setting the Variables

The Mobile.BuildTools attempts to locate the values for your Manifest tokens through several sources. In the event that a variable key is duplicated, the Mobile.BuildTools has a precedence that the last one in wins. Variables are loaded from the following sources:

1. buildtools.json Environment Defaults
1. buildtools.json Environment Configuration (i.e. Debug, Release)
1. System Environment
1. Recursively load legacy `secrets.json` from the Project directory to the Solution directory
1. Recursively load `appsettings.json` from the Project directory to the Solution directory
