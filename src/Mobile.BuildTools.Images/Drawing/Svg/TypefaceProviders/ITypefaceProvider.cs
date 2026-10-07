#nullable enable
// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See ../LICENSE.Svg.Skia.txt.
// Adapted from Svg.Skia v5.2.3; see ../PROVENANCE.md.
namespace Mobile.BuildTools.Drawing.Svg.TypefaceProviders;

internal interface ITypefaceProvider
{
    SkiaSharp.SKTypeface? FromFamilyName(string fontFamily, SkiaSharp.SKFontStyleWeight fontWeight, SkiaSharp.SKFontStyleWidth fontWidth, SkiaSharp.SKFontStyleSlant fontStyle);
}

