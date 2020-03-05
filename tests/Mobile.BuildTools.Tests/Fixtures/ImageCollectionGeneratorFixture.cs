using System.IO;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Images;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures
{
    public abstract class ImageCollectionGeneratorFixture : FixtureBase
    {
        protected static readonly string ImageDirectory = Path.Join("Templates", "Images");
        protected readonly string PlatformImageDirectory;
        protected readonly string DebugImageDirectory = Path.Join(ImageDirectory, "Debug");
        // 2 images with 2 generated configs
        protected const int ImageCount = 2 + 2;
        protected int PlatformOffset = 0;

        protected ImageCollectionGeneratorFixture(string platformImageDirectory, ITestOutputHelper testOutputHelper)
            : this(platformImageDirectory, null, testOutputHelper)
        {
        }

        protected ImageCollectionGeneratorFixture(string platformImageDirectory, string projectDirectory, ITestOutputHelper testOutputHelper)
            : base(projectDirectory, testOutputHelper)
        {
            PlatformImageDirectory = platformImageDirectory;
        }

        [Fact]
        public void GeneratorLocatesCorrectNumberOfAssetsImageDirectory()
        {
            var config = GetConfiguration();
            var generator = CreateGenerator(config, ImageDirectory);
            generator.Execute();

            Assert.Equal(ImageCount, generator.ImageInputFiles.Count);
        }

        [Fact]
        public void GeneratorLocatesCorrectNumberOfAssetsWithDebugDirectory()
        {
            var config = GetConfiguration();
            var generator = CreateGenerator(config, ImageDirectory, DebugImageDirectory);
            generator.Execute();

            Assert.Equal(ImageCount + 2, generator.ImageInputFiles.Count);
        }

        [Fact]
        public void GeneratorLocatesCorrectNumberOfAssetsWithPlatformDirectory()
        {
            var config = GetConfiguration();
            var generator = CreateGenerator(config, ImageDirectory, PlatformImageDirectory);
            generator.Execute();

            Assert.Equal(ImageCount + 2 + PlatformOffset, generator.ImageInputFiles.Count);
        }

        [Fact]
        public void GeneratorLocatesCorrectNumberOfAssetsWithPlatformAndDebugDirectory()
        {
            var config = GetConfiguration();
            var generator = CreateGenerator(config, ImageDirectory, DebugImageDirectory, PlatformImageDirectory);
            generator.Execute();

            Assert.Equal(ImageCount + 3 + 2 + PlatformOffset, generator.ImageInputFiles.Count);
        }

        [Fact]
        public void GeneratesJsonDefinition()
        {
            var config = GetConfiguration();
            var generator = CreateGenerator(config, ImageDirectory);
            generator.Execute();

            Assert.True(File.Exists(Path.Combine(ImageDirectory, "dotnetbot.json")));
            Assert.True(File.Exists(Path.Combine(ImageDirectory, "programmer-clipart.json")));
        }

        [Fact]
        public void DoesNotThrowException()
        {
            var config = GetConfiguration();
            var generator = CreateGenerator(config, ImageDirectory);
            var ex = Record.Exception(() => generator.Execute());
            Assert.Null(ex);
        }

        internal abstract ImageCollectionGeneratorBase CreateGenerator(IBuildConfiguration config, params string[] searchFolders);
    }
}
