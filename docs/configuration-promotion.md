# Configuration promotion: source checkpoint, not a signed-app proof

## Inventory

`Mobile.BuildTools.Configuration` already reads replaceable XML app.config assets.
`Mobile.BuildTools.Configuration.MSBuild` already transforms app.config using
app.{AppConfigEnvironment}.config, defaulting to the build Configuration. Inputs
can be project-local or linked MobileBuildToolsConfig items. Strategies control
which extra environment transforms are bundled. Production should normally bundle
only the final transformed app.config, with no credentials in it.

The separate existing `dansiegel/Mobile.BuildTools.Cli` repository is the CLI home.
Its original repack command was a placeholder and Program did not execute commands.
Do not create another CLI project.

This checkpoint adds a configuration-only MSBuild entry point:

```sh
dotnet msbuild path/to/existing-app.csproj -t:GenerateMobileBuildToolsConfiguration \
  -p:Configuration=Release -p:TargetFramework=net10.0-android \
  -p:AppConfigEnvironment=Production -p:RequireAppConfigTransform=true \
  -p:MobileBuildToolsConfigurationOutput=/absolute/new/production.config
```

The project must have been restored and the Configuration build tasks available.
This target invokes configuration tasks, not Compile/Build/Publish. It is not a
replacement for preparing the SDK/tasks. Per-project environment mappings remain
normal MSBuild property/item conditions or linked config items, not another CLI
mapping language. A required missing transform fails before deleting generated
config output. Optional base-only builds no longer dereference a missing transform.
Failed XDT transforms now fail instead of silently producing a deployable file.

## Actual platform matrix and gaps

| Framework target | Runtime source today | Build/runtime gap to close |
| --- | --- | --- |
| MAUI / Uno Android | Android AssetManager, root app.config | Prove Configuration-only package AndroidAsset injection on modern SDK; existing legacy umbrella target is insufficient evidence |
| MAUI / Uno iOS | NSBundle root app.config | Prove BundleResource logical path, release trimming and real IPA readback |
| MAUI Mac Catalyst / native .NET macOS | Apple bundle reader | Distinguish flat mobile bundle from Contents/Resources native macOS bundle; validate on existing app |
| MAUI / Uno Windows App SDK (WinUI) | Package.InstalledLocation, falling back to AppContext.BaseDirectory | Prove Content copy and packaged/unpackaged lookup, MSIX packaging |
| Uno Skia Desktop Windows/macOS/Linux | Generic net target currently uses current working directory | Use deployment-root/explicit package loader, never infer config from shell CWD; verify Uno asset mapping |
| Uno browser / WASM | No browser-specific async load path | Add asynchronous package-asset initialization; no synchronous HTTP or guessed local filesystem path |
| Samsung MAUI Tizen | Tizen resource reader exists | TFM/package output and Resource injection not validated; retain as separate support gate |

MAUI's supported target list: https://learn.microsoft.com/en-us/dotnet/maui/supported-platforms?view=net-maui-10.0
Uno's target list: https://platform.uno/docs/articles/getting-started/requirements.html
Uno package assets: https://platform.uno/docs/articles/features/file-management.html

Uno documents Content + StorageFile.GetFileFromApplicationUriAsync(ms-appx:///...)
across its targets; WASM loads the remote package asynchronously and caches it.
A generic asynchronous asset-reader abstraction should keep XML parsing and typed
configuration in the shared library and framework dependencies in thin adapters.
Do not claim NativeAOT compatibility from an ordinary build: XDT reflection and
runtime environment transforms need a trimming/AOT audit, with transformations
preferably performed at build/promotion time. Existing typed access must continue
reading runtime values, never generated environment constants baked into binaries.

## Smallest convincing proof (existing app only)

1. Use an existing sample/test app which reads Configuration. E2EApp and
   AppConfigSample already exist; the modern AppManifestsSample on the 3.0 work
   branch is another candidate after the separate 3.0 work lands. Do not add a new
   smoke/console app. Keep stable application identifier/version/entitlements and
   a visible configuration value such as Environment/ApiBaseUrl.
2. Build Release + QA once. Save artifact hash, executable entry hashes, manifests,
   resources, native compression methods/alignment, signing identity/entitlements,
   SDK versions, build log and elapsed time. Validate the QA app.
3. Run only the configuration-generation target with Production mapping, or supply
   a validated transformed config. Preserve the QA artifact unchanged.
4. Replace the intended package config, then run platform packaging/signing tools.
   Compare every unaffected entry, not just the main assembly. Verify config
   readback, app identity/entitlements, signatures, alignment and a runtime read of
   Production. For Mach-O compare executable semantics outside signature changes,
   since codesign may legitimately alter signature/link-edit bytes.
5. Time configuration generation, preparation, signing/verification separately and
   compare against the actual full Release build on the same machine/toolchain.
   No timing or signed-install claim exists until those real artifacts/tools run.

## Artifact-specific gates

- APK: assets/app.config; preserve payload bytes/compression, remove obsolete
  signatures, zipalign (including 16 KB native alignment) before apksigner, then
  verify signature/alignment and all unaffected content. Signing-ready does not
  mean install-verified. Changing certificate identity can break upgrades.
- AAB: base/assets/app.config only after module-aware inspection. Use bundletool
  validation and upload/JAR signing; Play generates/signs delivered APKs. Do not
  apply APK alignment or claim an AAB is an installable APK.
- IPA: inspect Payload/<app>.app and provisioning. Keep an original developer
  archive when export/distribution requires it. Mac codesign/export validation
  must preserve intended app IDs and entitlements across nested code. Changing
  distribution may require another profile/export rather than simply re-signing.
  Encrypted App Store downloads are not supported promotion inputs.
- Native macOS .app: inspect Contents/Info.plist, CFBundleExecutable and resource
  locations. Preserve symlinks, modes, frameworks, hardened-runtime flags and
  entitlements. Re-sign and validate notarization/stapling requirements separately.
- Windows: inspect loose app versus MSIX/AppX/installer. MSIX requires rebuilt
  block maps and package signing. A beside-executable config is only a hypothesis
  until the actual loader/package is verified.
- Linux: distinguish loose publish directories, AppImage, deb/rpm and containers.
  Their rebuilding/signing rules differ; never blindly unzip/repack them.
- Web/WASM: replace the deployed asset plus any content hashes/service-worker cache
  metadata through its deployment process. Browser caches must not silently retain
  QA config. This is not mobile binary signing.

## Verification status and deferred wrappers

Cloud CLI compilation and synthetic archive-orchestration checks are useful unit
coverage, not a substitute for the existing-app proof above. No Mac work was
resumed, no signing credential created or used intentionally for this workflow,
no package was published, and no CI job was added. Native SDK workloads, real
artifacts and platform signing tools are still needed for end-to-end validation.

After the MSBuild + CLI proof and eventual 3.0 publication, a thin GitHub Action
can wrap CLI inputs/results; existing Azure DevOps task stubs can become a second
thin wrapper. Both reuse core detection/generation/preparation/signing logic.
They are future work, not a prerequisite and not included in this checkpoint.
