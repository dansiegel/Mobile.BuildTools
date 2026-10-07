using System;
using System.IO;
using System.Linq;
using Mobile.BuildTools.Drawing.Svg;
using SkiaSharp;
using Xunit;
using LegacySvg = Svg.Skia.SKSvg;

namespace Mobile.BuildTools.Tests.Fixtures.Generators;

public sealed class OwnedSvgRendererFixture : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), nameof(OwnedSvgRendererFixture), Guid.NewGuid().ToString("N"));

    public OwnedSvgRendererFixture() => Directory.CreateDirectory(directory);

    [Fact]
    public void SolidRectangleHasExpectedDimensionsAndPixels()
    {
        using var svg = Load("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"8\" height=\"4\"><rect width=\"8\" height=\"4\" fill=\"#ff0000\"/></svg>");
        using var bitmap = Render(svg.Picture);
        Assert.Equal(8, bitmap.Width);
        Assert.Equal(4, bitmap.Height);
        for (var y = 0; y < bitmap.Height; y++)
            for (var x = 0; x < bitmap.Width; x++)
                Assert.Equal(SKColors.Red, bitmap.GetPixel(x, y));
    }

    [Fact]
    public void TransparentMarginsAndViewBoxScalingArePreserved()
    {
        using var svg = Load("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" viewBox=\"0 0 8 8\"><rect x=\"2\" y=\"2\" width=\"4\" height=\"4\" fill=\"blue\"/></svg>");
        using var bitmap = Render(svg.Picture);
        Assert.Equal(16, bitmap.Width);
        Assert.Equal(16, bitmap.Height);
        // RGB channels are unspecified when alpha is zero.
        Assert.Equal((byte)0, bitmap.GetPixel(0, 0).Alpha);
        Assert.Equal(SKColors.Blue, bitmap.GetPixel(8, 8));
        Assert.Equal((byte)0, bitmap.GetPixel(15, 15).Alpha);
    }

    [Fact]
    public void ReloadUsesChangedSourceAndDisposeIsIdempotent()
    {
        var path = Write("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"8\" height=\"4\"><rect width=\"8\" height=\"4\" fill=\"red\"/></svg>");
        var svg = new SvgImage();
        try
        {
            svg.Load(path);
            using (var first = Render(svg.Picture))
                Assert.Equal(SKColors.Red, first.GetPixel(2, 2));
            File.WriteAllText(path, File.ReadAllText(path).Replace("red", "blue"));
            svg.Load(path);
            using (var second = Render(svg.Picture))
                Assert.Equal(SKColors.Blue, second.GetPixel(2, 2));
        }
        finally
        {
            svg.Dispose();
            svg.Dispose();
        }
        Assert.Null(svg.Picture);
        Assert.Throws<ObjectDisposedException>(() => svg.Load(path));
    }

    [Fact]
    public void InternalXmlEntitiesKeepThePrimaryParsersBehavior()
    {
        const string content = "<!DOCTYPE svg [<!ENTITY color '#ff00ff'>]><svg xmlns=\"http://www.w3.org/2000/svg\" width=\"8\" height=\"4\"><rect width=\"8\" height=\"4\" fill=\"&color;\"/></svg>";
        AssertMatchesLegacy(Write(content));
        using var svg = Load(content);
        using var bitmap = Render(svg.Picture);
        Assert.Equal(SKColors.Magenta, bitmap.GetPixel(2, 2));
    }

    // The historical MBT dependency (Svg.Skia 2.0.0.1) ignored SMIL. The newer
    // test oracle evaluates time zero, so it is intentionally not used here.
    [Theory]
    [InlineData("<set attributeName=\"fill\" to=\"blue\" begin=\"0s\" dur=\"2s\"/>")]
    [InlineData("<animate attributeName=\"x\" from=\"2\" to=\"14\" begin=\"-1s\" dur=\"2s\"/>")]
    [InlineData("<animateTransform attributeName=\"transform\" type=\"translate\" from=\"0 0\" to=\"12 0\" begin=\"-1s\" dur=\"2s\"/>")]
    [InlineData("<animateMotion path=\"M 0 0 L 12 0\" begin=\"-1s\" dur=\"2s\"/>")]
    public void AnimationMarkupRetainsHistoricalStaticBaseImage(string animation)
    {
        using var svg = Load($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"20\" height=\"8\"><rect x=\"2\" y=\"2\" width=\"4\" height=\"4\" fill=\"red\">{animation}</rect></svg>");
        using var bitmap = Render(svg.Picture);
        Assert.Equal(SKColors.Red, bitmap.GetPixel(3, 3));
        Assert.Equal((byte)0, bitmap.GetPixel(10, 3).Alpha);
    }

    [Theory]
    [InlineData("<g transform=\"translate(3 2) rotate(12)\"><path d=\"M1 1 C20 0 2 25 25 20 Z\" fill=\"#33aaff\" stroke=\"#112233\" stroke-width=\"2\"/></g>")]
    [InlineData("<defs><linearGradient id=\"a\"><stop stop-color=\"red\"/><stop offset=\"1\" stop-color=\"blue\"/></linearGradient><radialGradient id=\"b\"><stop stop-color=\"white\"/><stop offset=\"1\" stop-color=\"green\"/></radialGradient></defs><rect width=\"32\" height=\"32\" fill=\"url(#a)\"/><circle cx=\"16\" cy=\"16\" r=\"10\" fill=\"url(#b)\"/>")]
    [InlineData("<defs><clipPath id=\"clip\"><circle cx=\"16\" cy=\"16\" r=\"11\"/></clipPath><mask id=\"mask\"><rect width=\"32\" height=\"32\" fill=\"white\"/><rect width=\"8\" height=\"32\" fill=\"black\"/></mask></defs><g opacity=\"0.6\" clip-path=\"url(#clip)\" mask=\"url(#mask)\"><rect width=\"32\" height=\"32\" fill=\"orange\"/></g>")]
    [InlineData("<style>.accent { fill: #8844cc; stroke: black; stroke-width: 1; }</style><defs><path id=\"shape\" d=\"M2 2 H12 V12 H2 Z\"/></defs><use xmlns:xlink=\"http://www.w3.org/1999/xlink\" xlink:href=\"#shape\" x=\"8\" y=\"7\" class=\"accent\"/>")]
    [InlineData("<defs><filter id=\"blur\"><feGaussianBlur stdDeviation=\"1.5\"/></filter></defs><rect x=\"8\" y=\"8\" width=\"16\" height=\"16\" fill=\"#0088ff\" filter=\"url(#blur)\"/>")]
    public void StaticDrawingFeaturesMatchLegacyPixels(string content) =>
        AssertMatchesLegacy(Write($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"32\" height=\"32\" viewBox=\"0 0 32 32\">{content}</svg>"));

    [Fact]
    public void TextShapingAndFontDiscoveryMatchLegacyPixels() =>
        AssertMatchesLegacy(Write("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"320\" height=\"80\"><rect width=\"320\" height=\"80\" fill=\"white\"/><text x=\"8\" y=\"52\" font-family=\"sans-serif\" font-size=\"36\" fill=\"black\">office ffi café</text></svg>"));

    [Theory]
    [InlineData("inline")]
    [InlineData("import")]
    [InlineData("linked")]
    [InlineData("media")]
    public void DocumentFontSourcesMatchLegacyPixels(string source)
    {
        var fontPath = FindRepositoryFile(Path.Combine("samples", "AppManifestsSample", "AppManifestsSample", "Resources", "Fonts", "OpenSans-Semibold.ttf"));
        var fontData = Convert.ToBase64String(File.ReadAllBytes(fontPath));
        var css = $"@font-face {{ font-family: MbtEmbeddedFixture; src: url('data:font/ttf;base64,{fontData}') format('truetype'); font-weight: 600; }}";
        File.WriteAllText(Path.Combine(directory, "font-face.css"), css);
        var prefix = source == "linked" ? "<?xml-stylesheet href=\"font-face.css\" type=\"text/css\" media=\"screen\"?>" : string.Empty;
        var style = source switch
        {
            "inline" => css,
            "import" => "@import url('font-face.css') screen;",
            "media" => $"@media screen {{ {css} }}",
            _ => string.Empty
        };
        const string drawing = "<text x=\"8\" y=\"52\" font-family=\"MbtEmbeddedFixture\" font-weight=\"600\" font-size=\"36\">office ffi café</text>";
        var content = $"{prefix}<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"320\" height=\"80\"><style>{style}</style>{drawing}</svg>";
        var path = Write(content);
        AssertMatchesLegacy(path);

        // Ensure the oracle actually loaded the document font instead of both
        // renderers silently falling back to a host-installed font.
        using var embedded = new LegacySvg();
        using var fallback = new LegacySvg();
        embedded.Load(path);
        fallback.Load(Write($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"320\" height=\"80\">{drawing}</svg>"));
        using var embeddedPixels = Render(embedded.Picture);
        using var fallbackPixels = Render(fallback.Picture);
        Assert.False(embeddedPixels.Bytes.SequenceEqual(fallbackPixels.Bytes), "The document-font fixture must differ from font fallback.");
    }

    [Fact]
    public void EmbeddedRasterImagesMatchLegacyPixels()
    {
        using var bitmap = new SKBitmap(2, 2);
        bitmap.Erase(SKColors.Red);
        bitmap.SetPixel(1, 1, SKColors.Blue);
        using var image = SKImage.FromBitmap(bitmap);
        using var png = image.Encode(SKEncodedImageFormat.Png, 100);
        var encoded = Convert.ToBase64String(png.ToArray());
        AssertMatchesLegacy(Write($"<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"16\" height=\"16\"><image x=\"2\" y=\"2\" width=\"12\" height=\"12\" xlink:href=\"data:image/png;base64,{encoded}\"/></svg>"));
    }

    [Theory]
    [InlineData("dotnetbot.svg")]
    [InlineData("logo.svg")]
    [InlineData("square.svg")]
    public void ExistingSvgCorpusMatchesLegacyPixels(string filename)
    {
        var relative = Path.Combine("tests", "Mobile.BuildTools.Tests", "Templates", "Images", filename);
        AssertMatchesLegacy(FindRepositoryFile(relative));
    }

    private string Write(string content)
    {
        var path = Path.Combine(directory, Guid.NewGuid().ToString("N") + ".svg");
        File.WriteAllText(path, content);
        return path;
    }

    private static string FindRepositoryFile(string relative)
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(root.FullName, relative)))
            root = root.Parent ?? throw new InvalidOperationException($"Regression input not found: {relative}");
        return Path.Combine(root.FullName, relative);
    }

    private SvgImage Load(string content)
    {
        var svg = new SvgImage();
        svg.Load(Write(content));
        return svg;
    }

    private static void AssertMatchesLegacy(string path)
    {
        using var expected = new LegacySvg();
        using var actual = new SvgImage();
        expected.Load(path);
        actual.Load(path);
        Assert.NotNull(expected.Picture);
        Assert.NotNull(actual.Picture);
        Assert.Equal(expected.Picture.CullRect, actual.Picture.CullRect);
        using var expectedPixels = Render(expected.Picture);
        using var actualPixels = Render(actual.Picture);
        Assert.Equal(expectedPixels.Bytes, actualPixels.Bytes);
    }

    private static SKBitmap Render(SKPicture picture)
    {
        var bitmap = new SKBitmap((int)Math.Ceiling(picture.CullRect.Width), (int)Math.Ceiling(picture.CullRect.Height));
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawPicture(picture);
        canvas.Flush();
        return bitmap;
    }

    public void Dispose() => Directory.Delete(directory, true);
}
