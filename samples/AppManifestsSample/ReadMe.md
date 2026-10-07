# Images and AppManifests sample

This .NET 10 MAUI sample reconciles the sample from PR #364. It uses the Images and AppManifests packages together. The SDK is not locked to a major version by `global.json`.

## Build from this checkout

Build the packages first, using an installed SDK that supports .NET 10:

```console
dotnet build src/Mobile.BuildTools.Core/Mobile.BuildTools.Core.csproj -c Release
dotnet build src/Mobile.BuildTools.AppManifests/Mobile.BuildTools.AppManifests.csproj -c Release
dotnet build src/Mobile.BuildTools.Images/Mobile.BuildTools.Images.csproj -c Release
```

The sample adds the repository's `Artifacts` directory through `RestoreAdditionalProjectSources`; no separate NuGet configuration is required. Set `MobileBuildToolsVersion` to the exact version produced by the package build (a single local AppManifests nupkg is selected automatically; multiple versions require an explicit selection). Use an isolated `RestorePackagesPath` when repeatedly rebuilding the same package version, so an older global-cache package cannot mask changes.

```console
dotnet build samples/AppManifestsSample/AppManifestsSample/AppManifestsSample.csproj -f net10.0-android -c Debug -p:MobileBuildToolsVersion=<exact-produced-version> -p:RestorePackagesPath=artifacts/sample-packages
```

This requires the MAUI Android workload and an Android SDK. With a running emulator, use the same command with `-t:Run`.

## Check the result

- Debug app identity: `com.avantipoint.mobilebuildtools.appmanifests.dev`
- Debug display name: `MBT AppManifests Dev`
- Android metadata `com.avantipoint.mobilebuildtools.API_HOST`: `dev-api.example.com`
- The main page displays the installed package identity and name
- The bot image has an MBT watermark and padding; MAUI owns the density-specific output generation
- Source AndroidManifest.xml and Info.plist still contain their tokens

Release changes the package identity, display name, API host, and cleartext-traffic setting according to `buildtools.json`. Inspect the final packaged Android manifest as well as the intermediate transformed source under `obj/.../Mobile.BuildTools/manifests`. Rebuild unchanged and verify image and manifest intermediate timestamps remain stable, then clean and rebuild.

## Other targets

The sample also declares iOS, Mac Catalyst, and Windows targets on their supported hosts. Apple transforms preserve framework/user partial manifests, including MAUI splash and font entries. Windows templates continue through MAUI's manifest transformer. These platforms require their native workloads; cloud target-contract tests are not full platform compilation or runtime validation.
