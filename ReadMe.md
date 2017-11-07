# Build Tools

As part of the continuing effort to provide better tooling for developers, I have created the Mobile.BuildTools. Mobile.BuildTools provides an easy way to handle a variety of DevOps tasks that stem from the necessity to protect sensitive or build defined conditions.

## Prism MFractor Config

As part of the DevOps tooling, you can now easily include an MFractor config to enable MFractor to better utilize Prism naming conventions. If you have modified your project structure you can always override individual settings by adding your own MFractor config. More information can be found in the [MFractor Docs](http://docs.mfractor.com/).

## Mobile.BuildTools

As part of the Build Tools, a number of Build Properties are added to better assist your DevOps pipeline by adding intelligence to the Build Process.

- IsWindows
- IsUnix (True on both macOS and Linux)
- *IsOSX
- *IsLinux
- IsiOSProject
- IsAndroidProject
- IsUWPProject
- IsMacOSProject
- IsTizenProject

**Notes:**

\* `IsOSX` and `IsLinux` can not be determined as true when the executing MSBuildRuntimeType is `Mono`.

### Secrets

Modern Apps have a lot of secrets, or at the very least variables that may change based on an environment. Secrets could be something that really is sensitive and should be kept out of Source Control such as an OAuth Client Id, or it could be something such as the URL to use for the Backend which may change between Development, Staging, and Production. The Build Tools allow you to handle these secrets with ease in your application through the use of a JSON file. 

### Tokenized Android &amp; iOS Configurations

The build tools allow you to exclude your `Info.plist` &amp; `AndroidManifest.xml` from Source Control. To do this of course you will need to add the file(s) you wish to exclude in your .gitignore, then you will need to add a Tokenized Template that will be copied to your project iOS or Android Project. While the Build Variable will always be set for the Info.plist and AndroidManifest.xml path's, the Info.plist will only be copied when `IsiOSProject` is true, and similarly the AndroidManifest.xml will only be copied when `IsAndroidProject` is true.

The Build Tools will only copy your template files to your iOS/Android project. You will need to add a build step to handle the tokenized values.

| Build Variable | Default Value |
| -------------- | ------------- |
| BuildResourcesDirectoryName | build |
| BuildResourcesDirRootPath | `SolutionDir`/`BuildResourcesDirectoryName` |
| InfoPlistTemplateName | BuildTemplateInfo.plist |
| BuildTemplateInfoPlistPath | `BuildResourcesDirRootPath`/`InfoPlistTemplateName` |
| InfoPlistPath | `MSBuildProjectDirectory`/Info.plist |
| AndroidManifestTemplateName | AndroidTemplateManifest.xml |
| BuildTemplateAndroidManifestPath | `BuildResourcesDirRootPath`/`AndroidManifestTemplateName` | AndroidManifestPath | `MSBuildProjectDirectory`/Properties/AndroidManifest.xml |
