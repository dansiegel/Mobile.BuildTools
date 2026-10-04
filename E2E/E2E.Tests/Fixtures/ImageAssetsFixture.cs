using System;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using E2E.Tests.Helpers;
using Xunit;

namespace E2E.Tests.Fixtures;

public sealed class ImageAssetsFixture
{
    [Fact]
    public void BuiltConsumerContainsSvgRasterizedByThePackagedNativeRuntime()
    {
        var pixels = ReadPixels("icon.png", 64, 32, out var bytesPerPixel);
        Assert.Equal(new byte[] { 255, 0, 0 }, pixels[(8 * bytesPerPixel)..(8 * bytesPerPixel + 3)]);
        Assert.Equal(new byte[] { 0, 0, 255 }, pixels[(48 * bytesPerPixel)..(48 * bytesPerPixel + 3)]);
        if (bytesPerPixel == 4)
        {
            Assert.Equal(255, pixels[8 * bytesPerPixel + 3]);
            Assert.Equal(255, pixels[48 * bytesPerPixel + 3]);
        }
    }

    [Fact]
    public void BuiltConsumerContainsSvgTextRenderedByThePackagedFontAndShapingRuntime()
    {
        var pixels = ReadPixels("text.png", 320, 80, out var bytesPerPixel);
        var darkPixels = Enumerable.Range(0, 320 * 80).Count(pixel =>
        {
            var offset = pixel * bytesPerPixel;
            return pixels[offset] < 128 && pixels[offset + 1] < 128 && pixels[offset + 2] < 128
                && (bytesPerPixel == 3 || pixels[offset + 3] == 255);
        });

        // Host fonts and antialiasing differ. Require visible glyphs, not a font-specific
        // snapshot. Svg.Skia's normal text path also invokes its native HarfBuzz shaper.
        Assert.InRange(darkPixels, 100, 10000);
        Assert.Equal(new byte[] { 255, 255, 255 }, pixels[..3]);
    }

    private static byte[] ReadPixels(string name, int width, int height, out int bytesPerPixel)
    {
        using var image = ConsumerResources.Open($"Outputs.{name}");
        using var contents = new MemoryStream();
        image.CopyTo(contents);
        var png = contents.ToArray();

        Assert.Equal(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, png[..8]);
        Assert.Equal("IHDR", Encoding.ASCII.GetString(png, 12, 4));
        Assert.Equal(width, BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(16, 4)));
        Assert.Equal(height, BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(20, 4)));
        Assert.Equal(8, png[24]); // Eight-bit RGB or RGBA; no test-side Skia dependency.
        Assert.Contains(png[25], new byte[] { 2, 6 });
        Assert.Equal(0, png[26]);
        Assert.Equal(0, png[27]);
        Assert.Equal(0, png[28]); // Non-interlaced.

        bytesPerPixel = png[25] == 6 ? 4 : 3;
        using var compressed = new MemoryStream();
        for (var offset = 8; offset < png.Length;)
        {
            var length = BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(offset, 4));
            var type = Encoding.ASCII.GetString(png, offset + 4, 4);
            if (type == "IDAT")
                compressed.Write(png, offset + 8, length);
            offset += length + 12;
        }

        compressed.Position = 0;
        using var decompressed = new ZLibStream(compressed, CompressionMode.Decompress);
        var stride = width * bytesPerPixel;
        var pixels = new byte[stride * height];
        for (var y = 0; y < height; y++)
        {
            var filter = decompressed.ReadByte();
            Assert.InRange(filter, 0, 4);
            decompressed.ReadExactly(pixels.AsSpan(y * stride, stride));
            for (var x = 0; x < stride; x++)
            {
                var index = y * stride + x;
                var left = x >= bytesPerPixel ? pixels[index - bytesPerPixel] : 0;
                var above = y > 0 ? pixels[index - stride] : 0;
                var upperLeft = y > 0 && x >= bytesPerPixel ? pixels[index - stride - bytesPerPixel] : 0;
                var predictor = filter switch
                {
                    1 => left,
                    2 => above,
                    3 => (left + above) / 2,
                    4 => Paeth(left, above, upperLeft),
                    _ => 0
                };
                pixels[index] = unchecked((byte)(pixels[index] + predictor));
            }
        }
        Assert.Equal(-1, decompressed.ReadByte());
        return pixels;
    }

    private static int Paeth(int left, int above, int upperLeft)
    {
        var predicted = left + above - upperLeft;
        var leftDistance = Math.Abs(predicted - left);
        var aboveDistance = Math.Abs(predicted - above);
        var upperLeftDistance = Math.Abs(predicted - upperLeft);
        if (leftDistance <= aboveDistance && leftDistance <= upperLeftDistance)
            return left;
        return aboveDistance <= upperLeftDistance ? above : upperLeft;
    }
}
