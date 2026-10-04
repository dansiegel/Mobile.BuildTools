# Mobile.BuildTools.AppManifests

Tokenized application manifests for .NET 10 MAUI and Uno Platform builds. Packages contain separate task hosts for .NET MSBuild (`net10.0`) and full-framework MSBuild (`net472`); the consuming app's target framework does not choose the task host.

Configure a literal delimiter and values in `buildtools.json`:

```json
{
  "manifests": { "token": "$$", "variablePrefix": "Manifest_", "missingTokensAsErrors": true },
  "environment": { "defaults": { "Manifest_AppName": "Example & Co" } },
  "automaticVersioning": { "behavior": "Off" }
}
```

Use `$$AppName$$` inside XML attributes/text, plist values, or JSON strings. Replacement values are literal: dollar signs and regex characters are not special, and XML/JSON serialization supplies the escaping. Plist integer, real, date, and data values are validated after substitution. Unresolved tokens warn or fail according to `missingTokensAsErrors`. Values and complete manifests are not logged, even in debug mode.

## Framework integration

- Android: transform `AndroidManifest` before `PrepareForBuild` and the first package-identity read.
- Apple: transform the main plist and all collected `PartialAppManifest` inputs, preserving order, metadata, and framework-generated splash/font entries.
- Windows: transform `AppxManifest` and the MAUI/Uno manifest inputs before their normal framework transforms. Reserved Windows template tokens remain for the framework to expand.
- Uno WebAssembly: transform the JSON file referenced by `WasmPWAManifestFile`, retaining its deployment metadata and framework icon generation. `AppManifest.js` is a separate JavaScript boot manifest and is not processed as JSON.

Sources are unchanged. Outputs are isolated beneath the target-framework/runtime's `obj/.../Mobile.BuildTools/manifests` directory, registered with `FileWrites`, and written only when contents change. Automatic versioning can intentionally change output; disable it for fixed application versions.

The shared MAUI sample is in `samples/AppManifestsSample`. Host/target-contract tests do not replace platform workload compilation or runtime validation.
