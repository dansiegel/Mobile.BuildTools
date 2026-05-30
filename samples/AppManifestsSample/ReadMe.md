# AppManifestsSample

.NET 10 MAUI sample for `Mobile.BuildTools.AppManifests`.

The app was generated with the .NET 10 `maui` template and then updated only to demonstrate Mobile.BuildTools manifest token replacement for Android and Apple heads.

## What this demonstrates

- `Platforms/Android/AndroidManifest.xml` contains tokens for:
  - `android:label="$$AppDisplayName$$"`
  - `android:usesCleartextTraffic="$$UsesCleartextTraffic$$"`
  - metadata value `$$ApiHost$$`
- `Platforms/Android/Resources/xml/network_security_config.xml` is referenced from the manifest to show that tokenized manifest attributes can point at regular Android resources.
- `Platforms/iOS/Info.plist` contains tokens for:
  - `CFBundleDisplayName`
  - custom `MBTApiHost`
- `buildtools.json` supplies Debug and Release values and sets `MBTPackageName`, which Mobile.BuildTools uses to update the Android package / Apple bundle identifier.

## Build from source

From the repository root, build the AppManifests package into `Artifacts` first:

```bash
dotnet build src/Mobile.BuildTools.AppManifests/Mobile.BuildTools.AppManifests.csproj -v minimal
```

Then restore/build this sample:

```bash
cd samples/AppManifestsSample
dotnet restore AppManifestsSample.slnx
dotnet build AppManifestsSample/AppManifestsSample.csproj -f net10.0-android -v minimal
```

The sample `NuGet.config` adds `../../Artifacts` as a local source and the sample project uses `Mobile.BuildTools.AppManifests` version `2.1.0-pre.*` so it resolves the package built from this checkout.

## Validating generated manifests

After an Android build, inspect:

```bash
AppManifestsSample/bin/Debug/net10.0-android/android/AndroidManifest.xml
AppManifestsSample/obj/Debug/net10.0-android/Mobile.BuildTools/AndroidManifest.xml
```

Expected Debug values include:

- package / bundle id: `com.avantipoint.mobilebuildtools.appmanifests.dev`
- display name: `MBT AppManifests Dev`
- API host: `dev-api.example.com`
- cleartext traffic: `true`

On macOS, build iOS with:

```bash
dotnet build AppManifestsSample/AppManifestsSample.csproj -f net10.0-ios -v minimal
```

On Windows, full iOS compilation is not available, but the plist source can still be XML-validated:

```bash
python - <<'PY'
import plistlib
with open('AppManifestsSample/Platforms/iOS/Info.plist', 'rb') as f:
    plistlib.load(f)
print('Info.plist is valid XML plist syntax')
PY
```
