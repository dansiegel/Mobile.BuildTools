# Lottie raw animation assets (v3)

The v3 Images implementation selects white-label Lottie JSON and packages raw assets. It does not install or select a runtime player. This feature is under development on `ds/lottie-assets`; older released packages do not support it. The v3 schemas in this branch are source artifacts until the documentation release publishes them.

Declare animations explicitly in the existing app project. Other JSON files and raster sidecars are not inferred to be animations:

```xml
<ItemGroup>
  <MobileBuildToolsLottie Include="BrandAssets/Shared/logo.json"
                         LogicalName="animations/logo.json" />
</ItemGroup>
```

```json
{
  "$schema": "../schemas/v3/buildtools.schema.json",
  "images": {
    "directories": ["BrandAssets/Shared"],
    "conditionalDirectories": {
      "BrandA": ["BrandAssets/BrandA"],
      "BrandB": ["BrandAssets/BrandB"]
    },
    "optimizeLottie": true
  }
}
```

Use the schema path relative to your file, or the published v3 URL once available. BrandA/BrandB are build configuration names; platform keys and negated configurations use the existing Images resolver. Selection uses explicit `BuildToolsImageSearchPath` entries in their given order, then configured directories in reverse priority (later matching conditional directories before shared directories), then the original file. `BuildToolsIgnoreDefaultSearchPath=true` with explicit paths excludes configured directories. Replacement matches the exact filename, including `.json`; it never converts an SVG or `.lottie` archive.

`LogicalName` is the stable packaged path across brands. If omitted, `Link`, then the original filename is used. Paths must be relative, without `.` or `..` segments. Keep brand source folders outside framework default raw-asset globs; declaring one animation does not remove unrelated files that your project explicitly packages. Avoid sharing animation filenames with raster sidecar JSON.

## Conservative optimization and raw escape hatch

Optimization is on by default, including an omitted `images` object. Set `images.optimizeLottie` to `false` in buildtools.json to copy the selected animation byte-for-byte, including a BOM, whitespace and line endings. Selection and packaging still run. This setting is not an MSBuild property and has no per-file override. `images.disable` remains the broader Images switch.

Enabled optimization validates JSON and removes only insignificant whitespace outside strings. Numeric lexemes (including precision, exponent spelling and negative zero), strings, property/array order, duplicate properties, names, IDs, markers, keyframes and unknown extensions remain intact. No floating-point round trip, metadata stripping, GIF/SVG conversion, gzip, Brotli or dotLottie output occurs. Minified bytes remain ordinary UTF-8 JSON. A UTF-8 BOM is removed only on the optimized path.

## Local companions and offline behavior

Local `assets[].u + assets[].p` image references and font `fPath` references are discovered from the selected JSON. They resolve relative to that selected brand's animation, never through other brands' search folders. Their bytes are copied unchanged into the same relative locations beneath the logical animation directory. Keep companion images in subdirectories, such as `images/`, so the legacy top-level raster directory scanner does not also select them.

Additional dependencies can be declared with relative, semicolon-separated `CompanionAssets` metadata:

```xml
<MobileBuildToolsLottie Include="BrandAssets/Shared/logo.json"
                       LogicalName="animations/logo.json"
                       CompanionAssets="images/custom.png;fonts/custom.ttf" />
```

Embedded data URIs remain unchanged. Remote references remain unchanged and produce warnings; the build never downloads them. Missing dependencies, unsafe paths, symbolic-link companion traversal and conflicting logical outputs fail with source-specific diagnostics. Local paths must not escape the selected source directory. Font family names without file paths require player/application font configuration.

Raw mode attempts dependency discovery but does not require valid JSON. If unusual raw bytes cannot be parsed, it warns and copies them unchanged; use `CompanionAssets` to declare dependencies explicitly. Raw mode does not bypass missing-companion or path-safety checks.

## Packaging and incremental builds

Prepared JSON and companions live in task-owned intermediates isolated by project, configuration, target framework and runtime. They become `MauiAsset` for MAUI, `AndroidAsset` for non-MAUI Android, `BundleResource` for non-MAUI Apple targets, and `Content` with copy metadata for Windows/Uno/WASM/other targets. Animation JSON never enters the resize/density pipeline. LogicalName, Link and TargetPath retain stable runtime names.

The task runs before raw item collection on every relevant build. Content fingerprints record the effective optimization option, selected sources, dependencies, configuration, metadata and task identity. True → false → true, brand changes and same-timestamp dependency changes are reflected without Clean. Identical bytes retain timestamps, missing outputs regenerate, and removed companion outputs are retired inside the task-owned directory. Generated files are registered in FileWrites for normal Clean; no-op builds still register framework assets.

## Validation boundaries

Executable tests live in the existing Mobile.BuildTools.Tests project: real configuration save/reload, generated schema contracts, lexical minification/raw bytes, dependencies, no-op and option switching, and MSBuild raw adapter/selection behavior. These do not establish native installation or visual fidelity in a particular player. No native player or device test is included. Test your chosen player's rendering and platform resource-loading API with representative animations.
