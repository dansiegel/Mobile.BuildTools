#nullable enable
// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE.Svg.Skia.txt in this directory.
// Adapted from Svg.Skia v5.2.3 SKSvgSettings; see PROVENANCE.md.
using System;
using System.Collections.Generic;
using Mobile.BuildTools.Drawing.Svg.TypefaceProviders;

namespace Mobile.BuildTools.Drawing.Svg;

// Only settings used by static picture conversion belong to this bridge.
internal sealed class SvgRenderSettings : IDisposable
{
    internal SkiaSharp.SKColorSpace SrgbLinear { get; } =
        SkiaSharp.SKColorSpace.CreateRgb(SkiaSharp.SKColorSpaceTransferFn.Linear, SkiaSharp.SKColorSpaceXyz.Srgb);

    internal SkiaSharp.SKColorSpace Srgb { get; } =
        SkiaSharp.SKColorSpace.CreateRgb(SkiaSharp.SKColorSpaceTransferFn.Srgb, SkiaSharp.SKColorSpaceXyz.Srgb);

    internal IList<ITypefaceProvider> TypefaceProviders { get; } = new List<ITypefaceProvider>
    {
        new FontManagerTypefaceProvider(),
        new DefaultTypefaceProvider()
    };

    internal IList<ITypefaceProvider>? DocumentTypefaceProviders { get; set; }

    internal bool EnableSvgFonts => true;
    internal bool EnableTextReferences => true;
    internal bool EnableFilterBackgroundInputs => true;
    internal bool EnableBrokenImagePlaceholders => true;

    public void Dispose()
    {
        SrgbLinear.Dispose();
        Srgb.Dispose();
    }
}
