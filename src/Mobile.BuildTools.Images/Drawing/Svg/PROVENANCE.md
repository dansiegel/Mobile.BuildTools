# Static SVG backend provenance

The backend conversion, asset loading, and font-provider files in this directory
are adapted from [Svg.Skia v5.2.3](https://github.com/wieslawsoltes/Svg.Skia/tree/v5.2.3),
by Wiesław Šoltés, under the MIT license retained in LICENSE.Svg.Skia.txt.
Keep that license in distributed packages containing this code.

SvgImage.cs is Mobile.BuildTools' static-picture entry point. It intentionally
omits upstream viewer, interaction, JavaScript, animation, and export APIs.
SVG parsing, scene compilation, and command records remain supplied by the
Svg.Custom, Svg.Model, Svg.SceneGraph, and ShimSkiaSharp packages. These packages
are not source-copied here and retain their own licenses.

## Adaptations

- Move implementation types into Mobile.BuildTools.Drawing.Svg and make them internal.
- Replace SKSvgSettings with the static-only SvgRenderSettings.
- Compile the bridge against Mobile.BuildTools' selected SkiaSharp/HarfBuzzSharp versions.
- Use public structural hashes instead of ShimSkiaSharp's internal mutation counters.
- Do not register assembly-resolution handlers or modify host/app binding configuration.

## Pinned upstream files

| Local source | Upstream source | Git blob SHA |
| --- | --- | --- |
| SvgRenderSettings.cs | [src/Svg.Skia/SKSvgSettings.cs](https://github.com/wieslawsoltes/Svg.Skia/blob/v5.2.3/src/Svg.Skia/SKSvgSettings.cs) | 819065a9b921214580829f0516fb5e404ae02179 |
| SkiaModel.Caching.cs | [src/Svg.Skia/SkiaModel.Caching.cs](https://github.com/wieslawsoltes/Svg.Skia/blob/v5.2.3/src/Svg.Skia/SkiaModel.Caching.cs) | e97d1daa2937daeac4e8f8ccb86fb8f07f0e728d |
| SkiaModel.TextShaping.cs | [src/Svg.Skia/SkiaModel.TextShaping.cs](https://github.com/wieslawsoltes/Svg.Skia/blob/v5.2.3/src/Svg.Skia/SkiaModel.TextShaping.cs) | a4e859be524592b218389119e824af6746fcde3b |
| SkiaModel.cs | [src/Svg.Skia/SkiaModel.cs](https://github.com/wieslawsoltes/Svg.Skia/blob/v5.2.3/src/Svg.Skia/SkiaModel.cs) | 7be8dded226d64df5a24c64ab6790e4cd3aabadc |
| SkiaSvgAssetLoader.Caching.cs | [src/Svg.Skia/SkiaSvgAssetLoader.Caching.cs](https://github.com/wieslawsoltes/Svg.Skia/blob/v5.2.3/src/Svg.Skia/SkiaSvgAssetLoader.Caching.cs) | bd8aa1d0975ca858a9fd88b17feba94f94fdab5c |
| SkiaSvgAssetLoader.DocumentFonts.cs | [src/Svg.Skia/SkiaSvgAssetLoader.DocumentFonts.cs](https://github.com/wieslawsoltes/Svg.Skia/blob/v5.2.3/src/Svg.Skia/SkiaSvgAssetLoader.DocumentFonts.cs) | 2bb8ea2b27ea15fae3623d01054381f0c517e9f6 |
| SkiaSvgAssetLoader.cs | [src/Svg.Skia/SkiaSvgAssetLoader.cs](https://github.com/wieslawsoltes/Svg.Skia/blob/v5.2.3/src/Svg.Skia/SkiaSvgAssetLoader.cs) | 6c3882015c8e8f37abb9f0d1620e0cc68dbf103d |
| TypefaceProviders/CustomTypefaceProvider.cs | [src/Svg.Skia/TypefaceProviders/CustomTypefaceProvider.cs](https://github.com/wieslawsoltes/Svg.Skia/blob/v5.2.3/src/Svg.Skia/TypefaceProviders/CustomTypefaceProvider.cs) | f48c5f503dd7c324af189d79570f0dca0f12d88f |
| TypefaceProviders/DefaultTypefaceProvider.cs | [src/Svg.Skia/TypefaceProviders/DefaultTypefaceProvider.cs](https://github.com/wieslawsoltes/Svg.Skia/blob/v5.2.3/src/Svg.Skia/TypefaceProviders/DefaultTypefaceProvider.cs) | 9403761b5d0769546f679559864260d594eb6fe6 |
| TypefaceProviders/DocumentFontTypefaceProvider.cs | [src/Svg.Skia/TypefaceProviders/DocumentFontTypefaceProvider.cs](https://github.com/wieslawsoltes/Svg.Skia/blob/v5.2.3/src/Svg.Skia/TypefaceProviders/DocumentFontTypefaceProvider.cs) | 3c992eb1b416f8cba48b34cd3a10bdde1f7e42c6 |
| TypefaceProviders/FontManagerTypefaceProvider.cs | [src/Svg.Skia/TypefaceProviders/FontManagerTypefaceProvider.cs](https://github.com/wieslawsoltes/Svg.Skia/blob/v5.2.3/src/Svg.Skia/TypefaceProviders/FontManagerTypefaceProvider.cs) | 201733f10c59ed7c0e5c694242084cf9a58efa88 |
| TypefaceProviders/ITypefaceProvider.cs | [src/Svg.Skia/TypefaceProviders/ITypefaceProvider.cs](https://github.com/wieslawsoltes/Svg.Skia/blob/v5.2.3/src/Svg.Skia/TypefaceProviders/ITypefaceProvider.cs) | d64980c50bc6c98a5193c5d43c8ec5ad8b1c9952 |
| TypefaceProviders/SharedTypefaceCache.cs | [src/Svg.Skia/TypefaceProviders/SharedTypefaceCache.cs](https://github.com/wieslawsoltes/Svg.Skia/blob/v5.2.3/src/Svg.Skia/TypefaceProviders/SharedTypefaceCache.cs) | ba8aa539e2483de53d3b1cb77930e8cee2017b16 |

## Validation requirements

Both the net472 full-MSBuild host and net10.0 dotnet host must load the actual
packed task, with no Svg.Skia.dll or Svg.Animation.dll dependency. Exercise
paths, transforms, gradients, clip paths, masks/filters, text shaping, font
fallback/document fonts, and embedded raster images. Native SkiaSharp and
HarfBuzzSharp still need the correct OS/architecture package layout.


## Public-API integration and scope

SvgCssFontSources.cs is an independently written compatibility adapter using
public DOM serialization, SvgDocumentCompatibilityLoader, and
SvgExternalResourceResolver. It does not copy Svg.Custom parser source, invoke
private reflection, or require an InternalsVisibleTo relationship. It collects
font styles from the already parsed document and valid prolog xml-stylesheet
instructions. The latter metadata-only pass handles SVGZ, ignores DTDs, never
resolves external entities, and stops at the first document element.

Stylesheet retrieval remains limited to local files and data URIs, gated by the
public parser's existing processing-mode/resource policy. HTTP stylesheet
retrieval is not added. Media conditions are evaluated by the same public
parser on a self-contained probe using the document's declared viewport.
The probe rejects text that could introduce another CSS rule or import.

This is a parity-focused static renderer, not a new full SVG/CSS implementation.
As in pinned upstream v5.2.3, stylesheet font URLs use the document/stylesheet
base URI; this adapter does not introduce a new xml:base interpretation for
style/link declarations. Import and media expansion are supported; other
at-rule bodies retain the pinned font scanner's behavior rather than adding
new @supports or @layer evaluation. Non-prolog xml-stylesheet instructions are
outside this adapter's supported metadata boundary. Animation/JavaScript,
viewer interaction, and export APIs are excluded. No animation evaluation is
implemented by this bridge.

The lifecycle partials clear document fonts, positioned text blobs, conversion
caches, retained shim-picture references, and per-instance font lookup state on
reload/disposal. Shared upstream font-provider caches retain their existing
bounded lifetime behavior.

## Local verification status

The staged sources passed lexical delimiter, namespace/accessibility, and
private-API/reference scans. These checks are not C# compilation or rendering
validation. No local .NET SDK was available for this implementation task.
The existing repository CI tests must establish compilation, pixel parity,
font/import/media behavior, repeated load/dispose behavior, and exact-package
native loading on both task hosts before this work is considered validated.

## Historical rendering contract

Before this modernization, Mobile.BuildTools used Svg.Skia 2.0.0.1. Its
SKSvg.Load path opens the document, builds the static drawable model, and
records a picture; it has no animation evaluator. Unsupported animation
elements are ignored by its drawable factory. Mobile.BuildTools then encodes
each generated raster as PNG.

Svg.Skia 5.2.3, temporarily adopted by this draft PR, adds time-zero SMIL
evaluation through Svg.Animation. That newly introduced behavior is not a
historical compatibility requirement. This owned bridge retains base-document
static rendering; it does not evaluate an animation timeline or produce GIFs.
Animation/GIF/Lottie support is separate follow-up work. The 5.2.3 test-only
oracle is used for static SVG cases, while independent pixel expectations cover
animation markup's historical base-image behavior.

Historical sources:
- https://github.com/wieslawsoltes/Svg.Skia/blob/2.0.0.1/src/Svg.Skia/SKSvg.Model.cs
- https://github.com/wieslawsoltes/Svg.Skia/blob/2.0.0.1/src/Svg.Model/SvgExtensions.IO.cs
- https://github.com/wieslawsoltes/Svg.Skia/blob/2.0.0.1/src/Svg.Model/Drawables/DrawableFactory.cs
