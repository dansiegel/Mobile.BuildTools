using System;
using System.Diagnostics;
using System.IO;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures.Utils
{
    public sealed class BuildToolsConfigFixture : FixtureBase
    {
        private static readonly string MockEmptyConfigPath = Path.Combine(Environment.CurrentDirectory, nameof(BuildToolsConfigFixture));

        public BuildToolsConfigFixture(ITestOutputHelper testOutputHelper)
            : base(MockEmptyConfigPath, testOutputHelper)
        {
        }

        [Fact]
        public void AppConfigIsNotNull()
        {
            var config = CreateConfig();

            Assert.NotNull(config.AppConfig);
        }

        [Fact]
        public void ArtifactCopyIsNotNull()
        {
            var config = CreateConfig();

            Assert.NotNull(config.ArtifactCopy);
        }

        [Fact]
        public void AutomaticVersioningIsNotNull()
        {
            var config = CreateConfig();

            Assert.NotNull(config.AutomaticVersioning);
        }

        [Fact]
        public void CssIsNotNull()
        {
            var config = CreateConfig();

            Assert.NotNull(config.Css);
        }

        [Fact]
        public void ImagesIsNotNull()
        {
            var config = CreateConfig();

            Assert.NotNull(config.Images);
        }

        [Fact]
        public void ManifestsIsNotNull()
        {
            var config = CreateConfig();

            Assert.NotNull(config.Manifests);
        }

        [Fact]
        public void AppSettingsIsNotNull()
        {
            var config = CreateConfig();

            Assert.NotNull(config.AppSettings);
        }

        [Fact]
        public void ReleaseNotesIsNotNull()
        {
            var config = CreateConfig();

            Assert.NotNull(config.ReleaseNotes);
        }

        private BuildToolsConfig CreateConfig()
        {
            var stackTrace = new StackTrace();
            var configuration = GetConfiguration(stackTrace.GetFrame(1).GetMethod().Name);
            CreateDummySolutionFile(configuration.IntermediateOutputPath);

            return ConfigHelper.GetConfig(configuration.IntermediateOutputPath);
        }

        private static void CreateDummySolutionFile(string directory) => File.Create(Path.Combine(directory, "test.sln"));
    }
}
