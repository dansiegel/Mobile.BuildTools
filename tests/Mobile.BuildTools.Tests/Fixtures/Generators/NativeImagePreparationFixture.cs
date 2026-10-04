using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Tests.Mocks;
using Mobile.BuildTools.Utils;
using SkiaSharp;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures.Generators
{
    public class NativeImagePreparationFixture : FixtureBase
    {
        public NativeImagePreparationFixture(ITestOutputHelper output) : base(output) { }

        [Theory]
        [InlineData("MauiImage")]
        [InlineData("MauiIcon")]
        [InlineData("MauiSplashScreen")]
        [InlineData("UnoImage")]
        [InlineData("UnoIcon")]
        [InlineData("UnoSplashScreen")]
        public void PreparesBrandedSourcesAndPreservesFrameworkMetadata(string kind)
        {
            var config = GetConfiguration();
            var source = Path.Combine(config.ProjectDirectory, "logo.png");
            var brand = Path.Combine(config.ProjectDirectory, "brand");
            Directory.CreateDirectory(brand);
            WriteImage(source, SKColors.Blue);
            WriteImage(Path.Combine(brand, "logo.png"), SKColors.Red);
            var item = Item(source, kind);
            foreach (var name in new[] { "Link", "BaseSize", "Resize", "TintColor", "Color", "ForegroundScale", "CustomMetadata" })
                item.SetMetadata(name, "original-" + name);
            var preparer = Preparer(config, new[] { brand });

            var result = preparer.Prepare(item);

            Assert.NotEqual(source, result.ItemSpec);
            Assert.Equal(SKColors.Red, ReadColor(result.ItemSpec));
            Assert.Equal(SKColors.Blue, ReadColor(source));
            foreach (var name in new[] { "Link", "BaseSize", "Resize", "TintColor", "Color", "ForegroundScale", "CustomMetadata" })
                Assert.Equal("original-" + name, result.GetMetadata(name));
            Assert.Equal(2, preparer.FileWrites.Count);
            Assert.All(preparer.FileWrites, file => Assert.True(File.Exists(file)));
            Assert.False(File.Exists(Path.ChangeExtension(source, ".json")));
            Assert.False(File.Exists(Path.Combine(brand, "logo.json")));
        }

        [Fact]
        public void PreservesUntransformedSvgAndPreparesForeground()
        {
            var config = GetConfiguration();
            var source = Path.Combine(config.ProjectDirectory, "icon.svg");
            var foreground = Path.Combine(config.ProjectDirectory, "foreground.svg");
            const string svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\"><rect width=\"16\" height=\"16\" fill=\"red\"/></svg>";
            File.WriteAllText(source, svg);
            File.WriteAllText(foreground, svg);
            var item = Item(source, "MauiIcon");
            item.SetMetadata("ForegroundFile", "foreground.svg");
            item.SetMetadata("ForegroundScale", "0.65");
            var preparer = Preparer(config);

            var result = preparer.Prepare(item);

            Assert.EndsWith(".svg", result.ItemSpec);
            Assert.Equal(svg, File.ReadAllText(result.ItemSpec));
            Assert.Equal(svg, File.ReadAllText(result.GetMetadata("ForegroundFile")));
            Assert.Equal("0.65", result.GetMetadata("ForegroundScale"));
            Assert.Equal(4, preparer.FileWrites.Count);
        }

        [Fact]
        public void NoOpPreservesOutputsAndTracksEveryDependencyByContent()
        {
            var config = GetConfiguration();
            var source = Path.Combine(config.ProjectDirectory, "logo.png");
            var watermark = Path.Combine(config.ProjectDirectory, "mark.png");
            var font = Path.Combine(config.ProjectDirectory, "custom.ttf");
            var sidecar = Path.ChangeExtension(source, ".json");
            var globalConfig = Path.Combine(config.ProjectDirectory, "buildtools.json");
            WriteImage(source, SKColors.Blue);
            WriteImage(watermark, SKColors.Red);
            File.WriteAllText(font, "font dependency");
            File.WriteAllText(globalConfig, "{}");
            File.WriteAllText(sidecar, "{\"width\":16,\"height\":16,\"watermark\":{\"sourceFile\":\"mark.png\",\"fontFile\":\"custom.ttf\"}}");
            var item = Item(source, "UnoImage");
            NativeImagePreparer Create() => new NativeImagePreparer(config, "net10.0-android36.0", "android-arm64", Array.Empty<string>(), globalConfig);
            var first = Create().Prepare(item);
            var stamp = first.ItemSpec + ".inputs";
            var oldTime = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            File.SetLastWriteTimeUtc(first.ItemSpec, oldTime);
            File.SetLastWriteTimeUtc(stamp, oldTime);

            var unchanged = Create().Prepare(item);

            Assert.Equal(first.ItemSpec, unchanged.ItemSpec);
            Assert.Equal(oldTime, File.GetLastWriteTimeUtc(first.ItemSpec));
            Assert.Equal(oldTime, File.GetLastWriteTimeUtc(stamp));
            foreach (var dependency in new[] { source, watermark, font, sidecar, globalConfig })
            {
                var before = File.ReadAllText(stamp);
                var timestamp = File.GetLastWriteTimeUtc(dependency);
                if (Path.GetExtension(dependency) == ".png")
                    WriteImage(dependency, SKColors.Green);
                else
                    File.AppendAllText(dependency, " ");
                File.SetLastWriteTimeUtc(dependency, timestamp);
                Create().Prepare(item);
                Assert.NotEqual(before, File.ReadAllText(stamp));
            }
            File.Delete(first.ItemSpec);
            Create().Prepare(item);
            Assert.True(File.Exists(first.ItemSpec));
        }

        [Fact]
        public void IsolatesFrameworkRuntimeAndConfigurationOutputs()
        {
            var config = GetConfiguration();
            var source = Path.Combine(config.ProjectDirectory, "logo.png");
            WriteImage(source, SKColors.Blue);
            var item = Item(source, "MauiImage");
            string Prepare(string tfm, string rid) => new NativeImagePreparer(config, tfm, rid, Array.Empty<string>(), null).Prepare(item).ItemSpec;
            var paths = new[] { Prepare("net10.0-android36.0", "android-arm64"), Prepare("net10.0-ios26.0", "ios-arm64"),
                Prepare("net10.0-ios26.0", "iossimulator-arm64"), Prepare("net10.0-browserwasm", "browser-wasm") };
            Assert.Equal(paths.Length, paths.Distinct().Count());
            config.BuildConfiguration = "Release";
            Assert.DoesNotContain(Prepare("net10.0-android36.0", "android-arm64"), paths);
        }

        [Fact]
        public void IgnoresOnlyConfiguredItems()
        {
            var config = GetConfiguration();
            var source = Path.Combine(config.ProjectDirectory, "logo.png");
            WriteImage(source, SKColors.Blue);
            File.WriteAllText(Path.ChangeExtension(source, ".json"), "{\"ignore\":true}");
            var preparer = Preparer(config);
            Assert.Null(preparer.Prepare(Item(source, "UnoImage")));
            Assert.Empty(preparer.FileWrites);
        }

        [Fact]
        public void AppliesNativePaddingAndWatermarkBeforeFrameworkSizing()
        {
            var config = GetConfiguration();
            var source = Path.Combine(config.ProjectDirectory, "logo.png");
            WriteImage(source, SKColors.Red);
            File.WriteAllText(Path.ChangeExtension(source, ".json"), "{\"width\":32,\"height\":16,\"scale\":0.5,\"padFactor\":2,\"padColor\":\"Blue\"}");
            var prepared = Preparer(config).Prepare(Item(source, "MauiSplashScreen"));
            using var bitmap = SKBitmap.Decode(prepared.ItemSpec);
            Assert.Equal(32, bitmap.Width);
            Assert.Equal(16, bitmap.Height);
            Assert.Equal(SKColors.Blue, bitmap.GetPixel(0, 0));
            Assert.Equal(SKColors.Red, bitmap.GetPixel(16, 8));
        }

        private static NativeImagePreparer Preparer(TestBuildConfiguration config, string[] folders = null) =>
            new NativeImagePreparer(config, "net10.0-android36.0", "android-arm64", folders ?? Array.Empty<string>(), null);

        private static TaskItem Item(string source, string kind)
        {
            var item = new TaskItem(source);
            item.SetMetadata("MobileBuildToolsItemType", kind);
            return item;
        }

        private static void WriteImage(string path, SKColor color)
        {
            using var bitmap = new SKBitmap(16, 16);
            bitmap.Erase(color);
            using var stream = File.Create(path);
            bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
        }

        private static SKColor ReadColor(string path)
        {
            using var bitmap = SKBitmap.Decode(path);
            return bitmap.GetPixel(8, 8);
        }
    }
}
