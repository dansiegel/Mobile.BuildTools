using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Tests.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures.Generators
{
    public class LottieAssetFixture : FixtureBase
    {
        public LottieAssetFixture(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void MinifiesOnlyWhitespacePreservingLexemesAndDuplicateProperties()
        {
            const string input = "{ \"n\": -0, \"n\": 1.234567890123456789e+003, \"text\": \"a \\\" \\\\ \\u0061 é\", \"markers\": [ { \"tm\": 2.00 } ], \"unknown\": true }";
            const string expected = "{\"n\":-0,\"n\":1.234567890123456789e+003,\"text\":\"a \\\" \\\\ \\u0061 é\",\"markers\":[{\"tm\":2.00}],\"unknown\":true}";
            Assert.Equal(expected, Encoding.UTF8.GetString(LottieJson.Minify(Encoding.UTF8.GetBytes(input))));
            Assert.ThrowsAny<JsonException>(() => LottieJson.Minify(Encoding.UTF8.GetBytes("{ bad }")));
        }

        [Fact]
        public void TrueFalseTruePreservesRawBomAndNoOpTimestamps()
        {
            var config = GetConfiguration();
            var source = Path.Combine(config.ProjectDirectory, "logo.json");
            var bytes = new byte[] { 0xef, 0xbb, 0xbf }.Concat(Encoding.UTF8.GetBytes("{\r\n \"nm\": \"logo\", \"layers\": []\r\n}\r\n")).ToArray();
            File.WriteAllBytes(source, bytes);
            var item = Item(source, "animations/logo.json");
            var first = Create(config).Prepare(new[] { item }).Single();
            Assert.Equal("{\"nm\":\"logo\",\"layers\":[]}", File.ReadAllText(first.ItemSpec));
            var time = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            File.SetLastWriteTimeUtc(first.ItemSpec, time);
            var noop = Create(config);
            Assert.Equal(first.ItemSpec, noop.Prepare(new[] { item }).Single().ItemSpec);
            Assert.Equal(time, File.GetLastWriteTimeUtc(first.ItemSpec));
            Assert.Contains(first.ItemSpec, noop.FileWrites);
            config.Configuration.Images.OptimizeLottie = false;
            var raw = Create(config).Prepare(new[] { item }).Single();
            Assert.Equal(first.ItemSpec, raw.ItemSpec);
            Assert.Equal(bytes, File.ReadAllBytes(raw.ItemSpec));
            config.Configuration.Images.OptimizeLottie = true;
            Assert.Equal("{\"nm\":\"logo\",\"layers\":[]}", File.ReadAllText(Create(config).Prepare(new[] { item }).Single().ItemSpec));
            File.Delete(first.ItemSpec);
            Assert.True(File.Exists(Create(config).Prepare(new[] { item }).Single().ItemSpec));
            Assert.Equal(bytes, File.ReadAllBytes(source));
        }

        [Fact]
        public void RawBypassAcceptsUnparseableInputAndExplicitCompanions()
        {
            var config = GetConfiguration();
            config.Configuration.Images.OptimizeLottie = false;
            var source = Path.Combine(config.ProjectDirectory, "logo.json");
            File.WriteAllBytes(source, new byte[] { 0xff, 0x00, 0x09 });
            File.WriteAllText(Path.Combine(config.ProjectDirectory, "image.png"), "opaque image bytes");
            var item = Item(source, "animations/logo.json");
            item.SetMetadata("CompanionAssets", "image.png");
            var prepared = Create(config).Prepare(new[] { item });
            Assert.Equal(2, prepared.Length);
            Assert.Equal(new byte[] { 0xff, 0x00, 0x09 }, File.ReadAllBytes(prepared.Single(x => x.GetMetadata("LogicalName").EndsWith("logo.json")).ItemSpec));
            Assert.Contains(prepared, x => x.GetMetadata("LogicalName") == "animations/image.png");
            config.Configuration.Images.OptimizeLottie = true;
            Assert.Throws<InvalidDataException>(() => Create(config).Prepare(new[] { item }));
        }

        [Fact]
        public void BrandDependenciesStayTogetherAndRemovedOutputsAreRetired()
        {
            var config = GetConfiguration();
            var source = Path.Combine(config.ProjectDirectory, "logo.json");
            File.WriteAllText(source, "{}");
            var brandA = Path.Combine(config.ProjectDirectory, "a");
            var brandB = Path.Combine(config.ProjectDirectory, "b");
            foreach (var brand in new[] { brandA, brandB })
            {
                Directory.CreateDirectory(Path.Combine(brand, "images"));
                File.WriteAllText(Path.Combine(brand, "logo.json"), "{ \"assets\": [{ \"u\": \"images/\", \"p\": \"pixel.png\" }], \"fonts\": {\"list\":[{\"fPath\":\"font.ttf\"}]} }");
                File.WriteAllText(Path.Combine(brand, "images", "pixel.png"), brand);
                File.WriteAllText(Path.Combine(brand, "font.ttf"), brand);
            }
            var item = Item(source, "animations/logo.json");
            var first = Create(config, brandA).Prepare(new[] { item });
            Assert.Equal(3, first.Length);
            var image = first.Single(x => x.GetMetadata("LogicalName") == "animations/images/pixel.png");
            Assert.Equal(brandA, File.ReadAllText(image.ItemSpec));
            var second = Create(config, brandB).Prepare(new[] { item });
            Assert.Equal(brandB, File.ReadAllText(image.ItemSpec));
            Assert.Equal(first.Select(x => x.ItemSpec), second.Select(x => x.ItemSpec));
            var dependency = Path.Combine(brandB, "images", "pixel.png");
            var stamp = File.GetLastWriteTimeUtc(dependency);
            File.WriteAllText(dependency, "changed without timestamp");
            File.SetLastWriteTimeUtc(dependency, stamp);
            Create(config, brandB).Prepare(new[] { item });
            Assert.Equal("changed without timestamp", File.ReadAllText(image.ItemSpec));
            File.WriteAllText(Path.Combine(brandB, "logo.json"), "{}");
            Assert.Single(Create(config, brandB).Prepare(new[] { item }));
            Assert.False(File.Exists(image.ItemSpec));
            Assert.Empty(Create(config).Prepare(Array.Empty<TaskItem>()));
            Assert.All(first, output => Assert.False(File.Exists(output.ItemSpec)));
        }

        [Fact]
        public void DiagnosesMissingConflictingAndUnsafeResourcesWithoutDownloading()
        {
            var config = GetConfiguration();
            var source = Path.Combine(config.ProjectDirectory, "logo.json");
            File.WriteAllText(source, "{\"assets\":[{\"u\":\"https://example.invalid/\",\"p\":\"never.png\"},{\"p\":\"data:image/png;base64,AAAA\"}]}");
            var item = Item(source, "animations/logo.json");
            Assert.Single(Create(config).Prepare(new[] { item }));
            item.SetMetadata("CompanionAssets", "missing.png");
            Assert.Throws<FileNotFoundException>(() => Create(config).Prepare(new[] { item }));
            item.SetMetadata("CompanionAssets", "../outside.png");
            Assert.Throws<InvalidDataException>(() => Create(config).Prepare(new[] { item }));
            item.SetMetadata("CompanionAssets", "");
            item.SetMetadata("LogicalName", "../outside.json");
            Assert.Throws<InvalidDataException>(() => Create(config).Prepare(new[] { item }));
            var other = Path.Combine(config.ProjectDirectory, "other.json");
            File.WriteAllText(other, "{}");
            Assert.Throws<InvalidDataException>(() => Create(config).Prepare(new[] { Item(source, "logo.json"), Item(other, "logo.json") }));
        }

        [Fact]
        public void FrameworkRuntimeAndConfigurationAreIsolated()
        {
            var config = GetConfiguration();
            var source = Path.Combine(config.ProjectDirectory, "logo.json");
            File.WriteAllText(source, "{}");
            var item = Item(source, "logo.json");
            string Output(string tfm, string rid) => new LottieAssetPreparer(config, tfm, rid, Array.Empty<string>(), null).Prepare(new[] { item }).Single().ItemSpec;
            var paths = new[] { Output("net10.0-android", "android-arm64"), Output("net10.0-ios", "ios-arm64"), Output("net10.0-ios", "iossimulator-arm64") };
            Assert.Equal(3, paths.Distinct().Count());
            config.BuildConfiguration = "Release";
            Assert.DoesNotContain(Output("net10.0-android", "android-arm64"), paths);
        }

        private static LottieAssetPreparer Create(TestBuildConfiguration config, params string[] folders) =>
            new LottieAssetPreparer(config, "net10.0-android", "android-arm64", folders, null);
        private static TaskItem Item(string source, string name)
        {
            var item = new TaskItem(source);
            item.SetMetadata("LogicalName", name);
            return item;
        }
    }
}
