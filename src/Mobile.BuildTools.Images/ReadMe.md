# Mobile.BuildTools.Images

Prepares branded image sources for .NET 10 MAUI and Uno Platform applications, with optional JSON-configured padding, backgrounds, and watermarks. MAUI and Uno own density generation, adaptive icons, splash resources, and platform asset catalogs.

## Native items

Existing `MauiImage`, `MauiIcon`, `MauiSplashScreen`, `UnoImage`, `UnoIcon`, and `UnoSplashScreen` items are prepared before framework item collection and export. Item metadata such as `Link`, `BaseSize`, `Resize`, `Color`, `TintColor`, and `ForegroundScale` is retained. `ForegroundFile` is redirected to its prepared source. Native logical names and extra outputs remain controlled by framework items; legacy `name` and `additionalOutputs` settings do not create native items.

Sources and optional same-name `.json` sidecars are selected from `BuildToolsImageSearchPath`, then matching build-configuration and platform `images.conditionalDirectories`, then shared `images.directories`, then the original source directory. Later configured directories override earlier directories; explicit search paths are searched in their given order. `BuildToolsIgnoreDefaultSearchPath=true` limits branded lookup to explicit search paths. Relative watermark and font paths can resolve from the sidecar directory, image search directories, or the project directory.

Unchanged SVGs remain vector sources. Width/height overrides, background colors, padding, and watermarks render a PNG for the framework to consume. Explicit dimensions take precedence over scale, and a single dimension preserves the source aspect ratio. Padding changes the content area within the requested final dimensions. Source-image scaling does not distort watermark placement.

Prepared sources and input fingerprints live under the intermediate output directory, isolated by configuration, target framework, runtime, and project. Content fingerprints include source/sidecar/configuration, watermark, font, item metadata, and task assembly. Unchanged builds preserve timestamps; missing outputs are regenerated. All generated files are registered as `FileWrites`. Preparation never creates or updates source sidecars. Uno Android design-time splash/resource generation remains enabled.

Projects without native MAUI/Uno image items retain the legacy platform image generation path. Task packages support both .NET 10 MSBuild and full-framework MSBuild hosts.

## Linux build hosts

SVG text and font-family watermarks use system font discovery. Linux build hosts need fontconfig (`libfontconfig.so.1`) and the fonts used by their images installed. The package includes the fontconfig-enabled Skia native runtime so SVG text and font-family watermarks can resolve those fonts. A configured watermark `fontFile` is the most predictable choice across hosts.

## App icon and splash manifest references

When image processing is enabled, native `MauiIcon` / `UnoIcon` declarations (or `MauiImage` / `UnoImage` with `IsAppIcon="true"`) also supply missing app-icon manifest references. This works with the Images package alone; AppManifests may additionally transform the generated inputs. Source manifests are never edited. Explicit references are preserved, with a build warning when they select something different from the generated icon.

- Android: an intermediate manifest fills missing `android:icon="@mipmap/{name}"` and `android:roundIcon="@mipmap/{name}_round"`. These names match the adaptive XML mipmaps produced by MAUI and Uno, including a single-source icon without a separate foreground. If either explicit reference selects another icon, neither missing counterpart is added, avoiding a mixed pair.
- iOS / Mac Catalyst: a `PartialAppManifest` with `Overwrite="false"` selects `Assets.xcassets/{name}.appiconset` via `XSAppIconAssets`. The .NET Apple SDK consumes this plural key for both MAUI and Uno and lets actool generate the final bundle-icon entries. Existing source/partial selections and `AppManifestEntry` values take precedence. Existing manually declared bundle-icon keys are preserved instead of selecting another catalog.
- Apple splash screens: framework-generated storyboard and splash plist inputs are retained. No duplicate launch-storyboard key is emitted. Mac Catalyst does not get an invented iOS launch storyboard.
- Windows: MAUI / Uno own their generated appx manifest's icon and splash references and packaged content. The Images package does not duplicate these transforms.
- Uno WebAssembly: Uno owns PWA `icons` generation from `WasmPWAManifestFile` when its `icons` array is missing or empty. Existing PWA icons remain explicit selections. The framework's splash `AppManifest.js` is separate from the PWA JSON and is never rewritten by Images.
- Android splash screens: the frameworks generate splash resources. Launcher activity/theme selection remains the application's responsibility, because a splash image alone does not identify the activity or authorize replacing its theme. Existing MAUI / Uno application templates already wire the appropriate launcher theme.

Names come from the resizetizer's `Link` alias when supplied, otherwise the prepared source filename; branded replacement paths never become manifest references. Legacy `resourceType: Mipmap` describes an Android resource folder, not a unique application-icon selection. Legacy Apple generation selects an existing `.appiconset` by name, and Windows `SplashScreen` / tile resource types describe legacy outputs. These ambiguous legacy settings are not promoted to new native icon or splash declarations; use explicit framework items for the automatic manifest bridge. Native iOS / Mac Catalyst are covered; legacy Xamarin, standalone macOS / tvOS, and desktop-specific icon metadata are not inferred.

Framework contracts: [MAUI icon configuration](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/images/app-icons?view=net-maui-10.0), [Uno image setup](https://platform.uno/docs/articles/external/uno.resizetizer/doc/using-uno-resizetizer.html), and the [.NET Apple manifest reader](https://github.com/dotnet/macios/blob/main/msbuild/Xamarin.MacDev.Tasks/Tasks/ReadAppManifest.cs).

## Static SVG renderer

The Skia-facing static SVG adapter is maintained in this project and compiled against its selected SkiaSharp version. Independent Svg.Custom, Svg.Model, Svg.SceneGraph, and ShimSkiaSharp parser/model packages remain managed dependencies. Svg.Skia and Svg.Animation are not part of the shipped package graph. SkiaSharp and HarfBuzzSharp native assets remain required for raster rendering and text shaping.

Adapted renderer and font-provider source retains its upstream MIT notices and pinned provenance in the third-party notices shipped with the Images package. The non-packable test project retains Svg.Skia as a pixel-parity oracle alongside independent size/color expectations. CI inspects produced packages separately and exercises SVG/native rendering with both .NET and full-framework MSBuild hosts.

As in the pre-modernization Svg.Skia 2.0.0.1 implementation, MBT rasterizes the static base document. Animation markup is not evaluated as a timeline, and MBT does not produce animated GIFs. The temporary 5.2.3 dependency added time-zero SMIL evaluation, which is deliberately not adopted as a new feature here. Animated SVG/GIF/Lottie support is separate follow-up work. Unchanged vector items continue through the framework-owned resizetizer.
