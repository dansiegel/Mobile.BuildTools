#nullable enable
namespace Mobile.BuildTools.Drawing.Svg;

internal partial class SkiaModel
{
    // Native picture/paint objects keep their own native references. After the
    // owned output picture is released, discard per-image conversion state and
    // explicitly release the positioned text blobs tracked by the upstream cache.
    internal void ClearStaticRenderState()
    {
        ClearCachedPictures();
        ClearReusableRenderCaches();
        ClearPositionedTextCache();
        _shapedTextCache = null;
        _shapedTextLayoutCache.Clear();
        _lastConvertedPicture = null;
        _previousConvertedPicture = null;
        _cacheShapedTextBlobsForCurrentPicture = false;
        _cacheComplexRenderPaintsForCurrentPicture = false;
        _typefaceCache.Clear();
        _resolvedTypefaceCache.Clear();
        _providerStateList = null;
        _providerStateHash = 0;
    }
}
