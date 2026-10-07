#nullable enable
using System;
using SkiaSharp;
using global::Svg.Model.Services;
using global::Svg.Skia;

namespace Mobile.BuildTools.Drawing.Svg;

// Owns one static SVG picture. Parsing and scene compilation remain independent
// managed dependencies; only this bridge references the selected native backend.
internal sealed class SvgImage : IDisposable
{
    private readonly SvgRenderSettings _settings = new();
    private readonly SkiaModel _model;
    private readonly SkiaSvgAssetLoader _assetLoader;
    private bool _disposed;

    public SvgImage()
    {
        _model = new SkiaModel(_settings);
        _assetLoader = new SkiaSvgAssetLoader(_model);
    }

    public SKPicture? Picture { get; private set; }

    public SKPicture? Load(string path)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SvgImage));
        if (path is null)
            throw new ArgumentNullException(nameof(path));

        Picture?.Dispose();
        Picture = null;
        _assetLoader.ClearStaticRenderState();
        _model.ClearStaticRenderState();

        var document = SvgService.Open(path);
        if (document is null)
            return null;

        // Keep embedded/CSS fonts alive through both layout and native recording.
        using var fontScope = _assetLoader.PushDocumentFonts(document);
        var pictureModel = SvgSceneRuntime.CreateModel(document, _assetLoader);
        Picture = _model.ToSKPicture(pictureModel);
        return Picture;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        Picture?.Dispose();
        Picture = null;
        _assetLoader.ClearStaticRenderState();
        _model.ClearStaticRenderState();
        _settings.Dispose();
    }
}
