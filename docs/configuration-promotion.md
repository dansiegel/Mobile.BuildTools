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

## Second source checkpoint

The feature branch now incorporates merged 3.0 master `dad3a1d`, preserving the
first WIP commit as history. Changes remain independent of the Lottie branch.

- `ConfigurationManager.InitAsync` accepts an async `(assetName, cancellationToken)
  reader returning a Stream. It requests app.config, owns/disposes that stream,
  snapshots the bytes, parses the final config and retains existing typed access.
  This is a framework-neutral seam, not automatic platform discovery. Call it
  before accessing Current on browser targets. It does not enable runtime XDT.
  Reset reuses the loaded snapshot; initialize again to reload an asset.
- MAUI callers can use FileSystem.Current.OpenAppPackageFileAsync; Uno callers can
  open `ms-appx:///app.config` via StorageFile and return its readable stream.
  Async platform APIs remain in application/framework code, avoiding a hard Uno or
  MAUI dependency in the shared Configuration library. These native/browser adapter
  calls have not been exercised here.
- Generic desktop lookup uses AppContext.BaseDirectory instead of process CWD.
  Apple missing-file handling returns an empty reader instead of constructing and
  discarding one. Base config parsing rejects DTDs and non-configuration roots.
- Modern SDK injection is now owned by the Configuration package: UnoSingleProject
  uses Content, MAUI uses MauiAsset, native Android uses AndroidAsset, Apple uses
  BundleResource, and desktop/WinUI use Content with output/publish copying.
  `MobileBuildToolsConfigurationAssetKind` can explicitly select one of those four
  modes for older/custom framework project shapes. Legacy umbrella injection is
  skipped for .NETCoreApp to avoid duplicate ownership. Actual SDK target ordering,
  output layout and packaged app readback remain validation gates.
- Configuration.MSBuild now directly references centrally versioned System.Text.Json
  rather than relying on its transitive Microsoft.Build dependency. This addresses
  the demonstrated #359 8.0-versus-10.0 resolution gap, but #359 must stay open until
  its real output and nupkg are inspected. Evidence from merged-master build:
  https://github.com/dansiegel/Mobile.BuildTools/actions/runs/37687969216/job/113020578895

Validation distinctions:

1. The existing Configuration runtime and existing Configuration.Tests projects
   compiled and ran 23 tests, including four new tests, in an isolated source-only
   check. That check used a temporary, uncommitted MSBuild import to exclude
   package-production references, use shared-framework Extensions assemblies, and
   cached Microsoft.NET.Test.Sdk 17.10.0 / xunit 2.9.3 / adapter 2.8.2. It did not
   create another project or app. It is NOT validation of the production package
   dependency graph and is not a substitute for the following failed check.
2. Restoring the original centrally versioned runtime/test graph from the available
   offline cache fails because Microsoft.Extensions.Configuration.Abstractions,
   Hosting and Hosting.Abstractions 10.0.12 are absent (nearest cache version
   10.0.11). No dependency versions were downgraded in source.
3. Restoring the actual net472 Configuration.MSBuild project resolves
   System.Text.Json/10.0.12 after the explicit reference, but the full restore fails
   for missing net472 reference assemblies and transitive packages including
   Microsoft.Bcl.AsyncInterfaces 10.0.12. A failed restore's selected version is
   diagnostic evidence only, not a successful package/build qualification.
4. No actual native/mobile/Uno/WASM build, NativeAOT publish, install, package
   signing, notarization or real preparation-vs-rebuild timing was performed.
   Configuration.MSBuild's legacy net472-only task packaging also still needs
   modern Core MSBuild host selection parity with the other task packages.
