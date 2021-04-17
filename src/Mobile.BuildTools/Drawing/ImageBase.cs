using System;
using System.Drawing;
using System.IO;
using SkiaSharp;

namespace Mobile.BuildTools.Drawing
{
    internal abstract class ImageBase : IDisposable
    {
        public ImageBase(string filename)
        {
            Filename = filename;
        }

        public string Filename { get; }

        public abstract bool HasTransparentBackground { get; }

        public int Height => GetOriginalSize().Height;

        public int Width => GetOriginalSize().Width;

        public abstract Size GetOriginalSize();

        public abstract void Draw(SKCanvas canvas, Context context);

        public static ImageBase Load(string filename)
            => IsVector(filename)
                ? new VectorImage(filename)
                : new Image(filename);

        private static bool IsVector(string filename) =>
            Path.GetExtension(filename)?.Equals(".svg", StringComparison.OrdinalIgnoreCase) ?? false;

        public virtual void Dispose()
        {

        }
    }
}
