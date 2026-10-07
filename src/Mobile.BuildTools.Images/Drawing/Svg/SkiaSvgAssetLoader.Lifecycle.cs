#nullable enable
namespace Mobile.BuildTools.Drawing.Svg;

internal partial class SkiaSvgAssetLoader
{
    internal void ClearStaticRenderState()
    {
        ClearDocumentFonts();
        _matchCharacterCache.Clear();
        _providerTypefaceCache.Clear();
        _typefaceSpanCache.Clear();
        _providerStateList = null;
        _providerStateHash = 0;
    }
}
