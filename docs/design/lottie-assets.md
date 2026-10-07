# Lottie asset support: exploration

!!! important
    This is an exploratory design, not an implemented feature or a statement of current package support. The proposed configuration and item declarations below are not usable yet. Preserve this work on `ds/lottie-assets`; merge only after the current Images SVG/version repair PR is merged. No runtime, production schema, project, dependency, or build target is changed by this document.

## Agreed scope

Support Lottie JSON in Mobile.BuildTools.Images for conditional inclusion of white-label animated logos and conservative build-time optimization. Reuse the existing branding model. Optimization is **on by default**, with an opt-out in **buildtools.json**, not an MSBuild property.

The proposed key is `images.optimizeLottie`. Setting it to `false` must still select the correct branded file and package it, while preserving that selected file byte-for-byte.

No player library, new project, gzip/Brotli output, dotLottie conversion, lossy transforms, or per-file optimization override is included in this scope.

## Proposed configuration and schema contract

This illustrative buildtools.json fragment is a proposal, not currently supported configuration:

```json
{
  "images": {
    "directories": ["Images/Shared"],
    "conditionalDirectories": {
      "BrandA": ["Images/BrandA"],
      "BrandB": ["Images/BrandB"]
    },
    "optimizeLottie": false
  }
}
```

BrandA and BrandB here are build configuration names, using existing conditional-directory semantics; they do not introduce an arbitrary brand-expression language. Both selected inputs can retain one logical runtime name such as `animations/logo.json`.

Proposed property schema within the existing ImageResize definition:

```json
"optimizeLottie": {
  "type": "boolean",
  "default": true,
  "description": "Minifies selected Lottie JSON assets. Enabled by default. Set false to preserve selected animation bytes while retaining conditional asset selection."
}
```

- Optional property. Omission means true; explicit true optimizes; explicit false copies unchanged.
- Null, strings, numbers, arrays and objects are invalid for this property.
- A schema default documents behavior; it does not initialize the runtime model.
- Add the property to `src/Mobile.BuildTools.Reference/Models/ImageResize.cs`, which is the configuration model for `images`.
- Initialize the runtime property to true and preserve explicit false during configuration serialization.
- Existing `images.disable` remains the broader Images-processing switch; optimization false must not be implemented as disable true.
- Update the production schema when implementing the feature, with executable schema/model tests and same-version docs/examples. Do not advertise support by modifying only a schema.

### Important existing-reader behavior

At baseline `bee5ee616bf60c5a992f6cbcef15afb0c51e2698`, `ConfigHelper.shared.cs` uses System.Text.Json Web defaults, permits comments and trailing commas, and creates missing configuration sections unless activation is skipped. It does not validate against JSON Schema while reading.

Its serializer uses `DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault`. A plain boolean property initialized to true would lose explicit false when SaveConfig omits the CLR default value, then reload as true. The implementation must override default omission for this property, for example with `JsonIgnore(Condition = JsonIgnoreCondition.Never)`, and test actual SaveConfig/GetConfig round trips. Test missing `images`, an empty images object, missing optimizeLottie, and explicit values through the real reader, not just direct deserialization. Consumers using skipActivation must still resolve the effective default to true.

Non-nullable boolean deserialization should reject invalid types; assert this through the actual reader and ensure task-level diagnostics identify the configuration path/property rather than silently falling back. Older packages do not implement this setting and must not be presented as supporting it.

The schema generator at `tools/Mobile.BuildTools.SchemaGenerator/Program.cs` uses Newtonsoft JSchemaGenerator while these models use System.Text.Json naming attributes. Verify emitted lowercase naming, optionality, boolean-only type and default explicitly. Do not assume the generator understands those attributes. The existing `ConfigurationSchemaFixture.cs` is commented out at the inspected baseline; writing more commented tests is not validation.

The repair checkpoint establishes the v3 package version baseline, but the inspected tree still contains v2 schemas. Confirm the release's schema-version convention before implementation. Either deliberately retain the existing schema URL with a documented additive update, or create the release-appropriate version and update references together. Never claim that an unpublished v3 schema URL already exists. Keep examples, schema, models and released package behavior aligned.

## Asset integration

Use an explicit declaration to distinguish animation JSON from unrelated JSON and raster-image sidecars. A dedicated `MobileBuildToolsLottie` item is a possible API, but final item naming and native raw-asset adapters remain to be validated.

Reuse the current image search resolver without changing its precedence:

1. Explicit BuildToolsImageSearchPath in its configured search order.
2. Matching configuration/platform conditional directories.
3. Shared image directories.
4. Original source directory.

Preserve existing later-directory override rules and BuildToolsIgnoreDefaultSearchPath behavior. Reuse this resolver; do not create a second subtly different interpretation of conditions.

Prepare selected files under the intermediate output directory with stable logical names. Feed raw-asset packaging, such as the appropriate MauiAsset, AndroidAsset, BundleResource or Uno content contract, after verifying that platform's actual collection target. Never feed animation JSON to MauiImage, raster generation, density scaling, watermark rendering or splash/icon inference. Preserve required logical-name/link metadata and prevent both original and prepared files from being packaged.

The first release should not infer every JSON file to be Lottie or auto-convert an existing .lottie archive. An explicit selected asset with optimization false takes the raw-copy path without requiring optimizer parsing.

### Referenced resources

Lottie JSON can reference local images through path/filename fields, embed images as data URIs, or refer to remote assets. Text can use fonts or embedded glyph data.

Maintain local paths relative to the selected branded source. Copy declared/discovered local dependencies into equivalent packaged locations, preserve embedded data, and include local dependency content in fingerprints. Resolve conflicts and missing local files with clear diagnostics. Do not download remote URLs implicitly or mix another brand's companion assets into the selected animation.

Dependency discovery must not defeat the raw compatibility escape hatch. If parsing an unusual input is unavailable with optimization false, require explicitly declared companion assets rather than rejecting or rewriting the animation. The concrete companion-item contract needs to be chosen and tested.

## Optimization boundary

Default optimization is validated, deterministic JSON minification only: remove insignificant whitespace outside string tokens while preserving numeric lexemes, strings, property/array ordering, unknown extensions and animation content. Avoid round-tripping numbers through floating-point models.

Do not remove names, IDs, markers, expressions, timing, keyframes, paths, glyphs or purported metadata. Runtime keypath APIs may use names. Duplicate properties must not be silently collapsed; preserve them or issue an explicit diagnostic rather than changing last/first-wins behavior.

Numeric precision reduction, keyframe/path simplification, image resizing/re-encoding and generic structural property stripping are separate, potentially lossy features. They require explicit future decisions and visual/player tests, not a more aggressive interpretation of this default.

When optimization is disabled, retain all selected input bytes, including whitespace, BOM and line endings. Do not deserialize/re-serialize, convert formats or write back to the source. Invalid JSON should produce a clear error when optimization requires validation; the raw bypass should still copy supplied bytes.

## Compression and player compatibility findings

- Minified JSON retains the underlying JSON format. SkiaSharp.Extended's documented MAUI Lottie source is a JSON file; that documentation does not establish compressed-format support.
- Gzip/Brotli change the stored representation and need a decoding path. Do not assume a local packaged .json.gz or .json.br is accepted by the chosen player.
- dotLottie is a defined Deflate ZIP container with a manifest and associated files, not a renamed gzip stream. V1 and V2 have different archive layouts and manifest contracts.
- Airbnb iOS documents DotLottieFile support. This is not proof that a .NET wrapper or every target/player version supports the same format.

There is no compressed output conversion in this proposal. Identify the actual consumer/player before considering it later. Report uncompressed input/output bytes if useful, but do not promise matching app-download savings or faster rendering: app packaging and animation complexity are separate factors.

## Deterministic and incremental behavior

Fingerprint selected source content and identity, resolved configuration, effective optimization flag, local dependencies, logical metadata and task version. Isolate intermediates by project, configuration, target framework and runtime, consistent with existing Images preparation.

Changing true to false must replace/reselect the packaged output even when the source timestamp is unchanged. The reverse switch must optimize again. Changes in brand selection, removed files, dependency content and options must invalidate appropriately; MSBuild input timestamps alone do not track changes to the input list.

Preserve output timestamps when bytes are unchanged; regenerate missing outputs; register generated files in FileWrites. Ensure framework items remain registered on skipped builds. Clean obsolete outputs safely within the task-owned intermediate area and prevent stale files from entering the application package.

## Implementation sequence after the current PR

1. Base the feature work on the merged repair result; rebase or cherry-pick this design commit as appropriate if the repair is squash-merged.
2. Add the boolean model/default/serialization contract and active schema tests. Decide the schema version convention and update the matching schema.
3. Add explicit Lottie asset identification, preparation and raw-asset mapping within the existing Images project. Reuse resolver/fingerprint infrastructure without altering image semantics.
4. Add conservative minification and byte-preserving opt-out, then dependency handling and incremental invalidation tests.
5. Add same-version configuration docs/examples and run existing Images/package/framework regression suites.
6. Validate a representative player fixture once the consumer is identified. Keep the feature in its own review; no merge before the current PR.

## Regression acceptance checklist

- [ ] Missing configuration section/property and explicit true optimize by default.
- [ ] Explicit false survives SaveConfig/GetConfig and copies byte-identical selected bytes, including BOM and line endings.
- [ ] Schema and actual reader reject null and non-boolean property values.
- [ ] Consecutive true/false/true builds work without Clean or stale packaged outputs.
- [ ] Conditional selection, platform/configuration precedence, explicit paths and ignore-default-path behavior match Images.
- [ ] All brands share a stable logical runtime name; only the selected brand's animation/resources enter the package.
- [ ] JSON numbers, escaped strings, Unicode, names, markers, unknown fields, nested assets and keyframe arrays retain semantics.
- [ ] Invalid optimization input fails clearly; raw bypass does not require optimizer parsing.
- [ ] Local referenced assets/fonts remain resolvable; missing/conflicting dependencies are diagnosed and no implicit network access occurs.
- [ ] No-op builds retain timestamps; missing outputs regenerate; dependency/config/tool changes invalidate.
- [ ] Item removal and brand switching remove stale package inclusion; FileWrites/Clean and skipped-target registration work.
- [ ] Existing PNG/SVG/icon/splash processing and raster sidecars remain unchanged.
- [ ] Schema generation output and every shipped configuration example are tested against the selected release schema.
- [ ] Existing package/framework suites pass; player smoke-test coverage and any unavailable target environments are reported accurately.

These are proposed acceptance criteria, not tests already run. The preservation commit contains documentation only.

## Primary references

- [Current Images behavior](../../src/Mobile.BuildTools.Images/ReadMe.md)
- [ImageResize configuration model](../../src/Mobile.BuildTools.Reference/Models/ImageResize.cs)
- [Configuration reader and serializer](../../src/Mobile.BuildTools.Reference/Utils/ConfigHelper.shared.cs)
- [Schema generator](../../tools/Mobile.BuildTools.SchemaGenerator/Program.cs)
- [Existing schema fixture](../../tests/Mobile.BuildTools.Tests/Fixtures/Configuration/ConfigurationSchemaFixture.cs)
- [Lottie asset references](https://lottiefiles.github.io/lottie-docs/assets/)
- [Lottie fonts and text](https://lottiefiles.github.io/lottie-docs/text/)
- [Airbnb dynamic properties and named keypaths](https://airbnb.gitbook.io/lottie/iosmacos/docs/dynamic-properties)
- [dotLottie specifications and version differences](https://dotlottie.io/spec/)
- [Airbnb iOS JSON and dotLottie loading](https://github.com/airbnb/lottie/blob/master/ios.md)
- [SkiaSharp.Extended Lottie documentation](https://github.com/mono/skiasharp.extended/blob/main/docs/docs/lottie.md)
- [MSBuild incremental builds](https://learn.microsoft.com/en-us/visualstudio/msbuild/incremental-builds?view=visualstudio)
