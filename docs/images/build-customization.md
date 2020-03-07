The image processing feature of the Mobile.BuildTools has always been built with the intention of making white labeling apps or customizing resources like app icons based on build environment easier. Because of this, in addition to configuring this feature with the buildtools.json, we also support overriding your default configuration at build using more conventional MSBuild properties.

## Customizing the Search Paths

In order to customize the search paths at build you should set the Property `BuildToolsImageSearchPath`. This property can contain multiple paths as long as the paths can be split with a semi-colon `;`.

Setting the property could be as simple as the following:

```xml
<PropertyGroup>
  <BuildToolsImageSearchPath>$(SolutionDir)\Images\MoreImages;$(SolutionDir)\Images\AwesomeImages</BuildToolsImageSearchPath>
</PropertyGroup>
```

## Overriding the Json Configuration

The Image Processing feature of the Mobile.BuildTools allows you to completely override your JSON configuration at build by specifying an additional property `BuildToolsIgnoreDefaultSearchPath`. By setting this property to true in the presence of any build defined search paths, we will ignore all paths including the conditional paths specified in the buildtools.json.

## Putting this together

To understand how we might use this in a DevOps environment we'll look at some YAML to see how this would look with Azure Pipelines:

```yaml
- task: XamariniOS@2
  displayName: 'Build Xamarin.iOS solution'
  inputs:
    solutionFile: 'src/AwesomeApp.iOS/AwesomeApp.iOS.csproj'
    configuration: ${{ parameters.buildConfiguration }}
    packageApp: true
    runNuGetRestore: true
    args: '/p:BuildToolsImageSearchPath=''$(ClientImages)'' /p:BuildToolsIgnoreDefaultSearchPath=true'
  env:
    Secret_SampleString: 'Sample String'
    Secret_SampleInt: '1'
    Secret_SampleDouble: '2.1'
    Secret_SampleBool: 'true'
```