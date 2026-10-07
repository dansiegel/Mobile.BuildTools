using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Configuration
{
    public sealed class LottieConfigurationFixture : IDisposable
    {
        private readonly string directory = Path.Combine(Path.GetTempPath(), "LottieConfig-" + Guid.NewGuid());
        public LottieConfigurationFixture() => Directory.CreateDirectory(directory);
        public void Dispose() => Directory.Delete(directory, true);

        [Theory]
        [InlineData("{}")]
        [InlineData("{\"images\":{}}")]
        [InlineData("{\"images\":{\"optimizeLottie\":true}}")]
        public void ActualReaderDefaultsOnIncludingSkipActivation(string json)
        {
            File.WriteAllText(Path.Combine(directory, "buildtools.json"), json);
            Assert.True(ConfigHelper.GetConfig(directory).Images.OptimizeLottie);
            Assert.True(ConfigHelper.GetConfig(directory, true).Images?.OptimizeLottie ?? true);
        }

        [Fact]
        public void FalseSurvivesActualSaveAndReload()
        {
            var file = Path.Combine(directory, "buildtools.json");
            File.WriteAllText(file, "{\"images\":{\"optimizeLottie\":false}}");
            var config = ConfigHelper.GetConfig(directory);
            Assert.False(config.Images.OptimizeLottie);
            ConfigHelper.SaveConfig(config, directory);
            using var saved = JsonDocument.Parse(File.ReadAllText(file));
            Assert.False(saved.RootElement.GetProperty("images").GetProperty("optimizeLottie").GetBoolean());
            Assert.False(ConfigHelper.GetConfig(directory).Images.OptimizeLottie);
        }

        [Theory]
        [InlineData("null")]
        [InlineData("\"false\"")]
        [InlineData("0")]
        [InlineData("[]")]
        [InlineData("{}")]
        public void ActualReaderAndGeneratedSchemaRejectNonBooleans(string value)
        {
            var json = "{\"images\":{\"optimizeLottie\":" + value + "}}";
            File.WriteAllText(Path.Combine(directory, "buildtools.json"), json);
            var error = Assert.Throws<JsonException>(() => ConfigHelper.GetConfig(directory));
            Assert.Contains("optimizeLottie", error.Path);
            Assert.False(JObject.Parse(json).IsValid(Mobile.BuildTools.SchemaGenerator.Program.CreateSchema(typeof(BuildToolsConfig))));
        }

        [Fact]
        public void CheckedInVersionThreeSchemasMatchGeneratorAndExamples()
        {
            var root = new DirectoryInfo(AppContext.BaseDirectory);
            while (!File.Exists(Path.Combine(root.FullName, "tools/Mobile.BuildTools.SchemaGenerator/Program.cs")))
                root = root.Parent ?? throw new InvalidOperationException("Repository not found.");
            foreach (var pair in new[] {
                (typeof(BuildToolsConfig), "buildtools.schema.json"),
                (typeof(Mobile.BuildTools.Models.AppIcons.ResourceDefinition), "resourceDefinition.schema.json") })
            {
                var generated = Mobile.BuildTools.SchemaGenerator.Program.CreateSchema(pair.Item1);
                var saved = File.ReadAllText(Path.Combine(root.FullName, "docs/schemas/v3", pair.Item2));
                Assert.True(JToken.DeepEquals(JToken.Parse(generated.ToString()), JToken.Parse(saved)));
            }
            var schema = Mobile.BuildTools.SchemaGenerator.Program.CreateSchema(typeof(BuildToolsConfig));
            foreach (var folder in new[] { "samples", "E2E", "tests/Mobile.BuildTools.Tests/Templates", "tests/Mobile.BuildTools.AppSettings.Tests/Sources" })
                foreach (var file in Directory.EnumerateFiles(Path.Combine(root.FullName, folder), "buildtools.json", SearchOption.AllDirectories)
                    .Where(path => !path.Split(Path.DirectorySeparatorChar).Any(part => part == "bin" || part == "obj")))
                    Assert.True(JObject.Parse(File.ReadAllText(file)).IsValid(schema, out System.Collections.Generic.IList<string> errors), file + ": " + string.Join("; ", errors));
        }

        [Fact]
        public void GeneratedSchemaUsesRuntimeNamesAndOptionalBooleanDefault()
        {
            var schema = Mobile.BuildTools.SchemaGenerator.Program.CreateSchema(typeof(BuildToolsConfig));
            Assert.Contains("images", schema.Properties.Keys);
            Assert.DoesNotContain("Images", schema.Properties.Keys);
            var images = schema.Properties["images"];
            Assert.Equal(JSchemaType.Boolean, images.Properties["optimizeLottie"].Type);
            Assert.True(images.Properties["optimizeLottie"].Default.Value<bool>());
            Assert.DoesNotContain("optimizeLottie", images.Required);
            foreach (var json in new[] { "{}", "{\"images\":{}}", "{\"images\":{\"optimizeLottie\":true}}", "{\"images\":{\"optimizeLottie\":false}}" })
                Assert.True(JObject.Parse(json).IsValid(schema));
        }
    }
}
