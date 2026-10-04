# Mobile.BuildTools.Images

Prepares branded image sources for .NET 10 MAUI and Uno Platform applications, with optional JSON-configured padding, backgrounds, and watermarks. MAUI and Uno own density generation, adaptive icons, splash resources, and platform asset catalogs.

## Native items

Existing `MauiImage`, `MauiIcon`, `MauiSplashScreen`, `UnoImage`, `UnoIcon`, and `UnoSplashScreen` items are prepared before framework item collection and export. Item metadata such as `Link`, `BaseSize`, `Resize`, `Color`, `TintColor`, and `ForegroundScale` is retained. `ForegroundFile` is redirected to its prepared source. Native logical names and extra outputs remain controlled by framework items; legacy `name` and `additionalOutputs` settings do not create native items.

Sources and optional same-name `.json` sidecars are selected from `BuildToolsImageSearchPath`, then matching build-configuration and platform `images.conditionalDirectories`, then shared `images.directories`, then the original source directory. Later configured directories override earlier directories; explicit search paths are searched in their given order. `BuildToolsIgnoreDefaultSearchPath=true` limits branded lookup to explicit search paths. Relative watermark and font paths can resolve from the sidecar directory, image search directories, or the project directory.

Unchanged SVGs remain vector sources. Width/height overrides, background colors, padding, and watermarks render a PNG for the framework to consume. Explicit dimensions take precedence over scale, and a single dimension preserves the source aspect ratio. Padding changes the content area within the requested final dimensions. Source-image scaling does not distort watermark placement.

Prepared sources and input fingerprints live under the intermediate output directory, isolated by configuration, target framework, runtime, and project. Content fingerprints include source/sidecar/configuration, watermark, font, item metadata, and task assembly. Unchanged builds preserve timestamps; missing outputs are regenerated. All generated files are registered as `FileWrites`. Preparation never creates or updates source sidecars. Uno Android design-time splash/resource generation remains enabled.

Projects without native MAUI/Uno image items retain the legacy platform image generation path. Task packages support both .NET 10 MSBuild and full-framework MSBuild hosts.
