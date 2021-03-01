# BuildToolsSample

This is a sample app showing how you might use some features of the Mobile.BuildTools. Note that you may have issues loading this in Visual Studio. It's best built from the command line in Visual Studio Code.

## Secrets

This uses several secrets that are defined in the buildtools.json. You might start by copying the following json into a file named secrets.json which can be placed either in the solution root directory (this directory) or in the project directory.

```json
{
  "AppDisplayName": "Build Tools",
  "Message": "Hello Mobile.BuildTools from secrets.json",
  "AppCenterId": "00000000-0000-0000-0000-000000000000",
  "Backend": "http://awesomebackend.azurewebsites.com",
  "ClientId": "00000000-0000-0000-0000-000000000000"
}
```

Note that secrets can be combined dynamically with a secrets.json in the solution root and in the project directory as well as by adding configuration based secrets like `secrets.Debug.json` which will conditionally be brought in. This will eventually support reuse across projects and app manifests like the Info.plist and AndroidManifest.xml.