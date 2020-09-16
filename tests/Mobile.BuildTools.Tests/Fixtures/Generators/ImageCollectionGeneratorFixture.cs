using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Images;
using Mobile.BuildTools.Models.AppIcons;
using Mobile.BuildTools.Utils;
using Xunit;
using Xunit.Abstractions;
using static Mobile.BuildTools.Tests.TestConstants;

namespace Mobile.BuildTools.Tests.Fixtures.Generators
{
    public abstract class ImageCollectionGeneratorFixture : FixtureBase
    {
        protected string PlatformImageDirectory => GetPlatformImageDirectory(Platform);

        // 2 images with 2 generated configs
        protected const int ImageCount = 2 + 2;
        protected int PlatformOffset = 0;
        protected abstract Platform Platform { get; }
        protected string PlatformIcon => GetPlatformIconPath(Platform);

        protected abstract Dictionary<string, IEnumerable<OutputImage>> ExpectedOutputs { get; }

        public ImageCollectionGeneratorFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            Initialize();
        }

        protected ImageCollectionGeneratorFixture(string projectDirectory, ITestOutputHelper testOutputHelper)
            : base(projectDirectory, testOutputHelper)
        {
            Initialize();
        }

        private void Initialize()
        {
            var rdInfo = new FileInfo(Path.Combine("resources", "resourceDefinition.json"));
            if (!rdInfo.Exists)
            {
                rdInfo.Directory.Create();
                _testOutputHelper.WriteLine("Adding resourceDefinition file");
                var assembly = GetType().Assembly;
                using var stream = assembly.GetManifestResourceStream("Mobile.BuildTools.Tests.resources.resourceDefinition.json");
                using var fs = rdInfo.Create();
                stream.CopyTo(fs);
            }
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

            foreach(var input in generator.ImageInputFiles)
            {
                _testOutputHelper.WriteLine(input);
            }
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
        public void HasExpectedOutputsForStandardImageImage()
        {
            var config = GetConfiguration();
            var generator = CreateGenerator(config, ImageDirectory);
           // generator.Execute();

            var resource = generator.GetResourceDefinition(Path.Combine(ImageDirectory, "dotnetbot.png"));
            var configs = resource.GetConfigurations(Platform);
            Assert.Single(configs);
            var imageResource = configs.First();

            Assert.Equal("dotnetbot", imageResource.Name);
            Assert.False(imageResource.Ignore);
            Assert.Equal(1, imageResource.Scale);
        }

        [Fact]
        public void IgnoresExampleWatermark()
        {
            var config = GetConfiguration();
            var generator = CreateGenerator(config, ImageDirectory, DebugImageDirectory);
           // generator.Execute();

            var resource = generator.GetResourceDefinition(Path.Combine(DebugImageDirectory, "example.png"));
            var configs = resource.GetConfigurations(Platform);
            Assert.Single(configs);
            var imageResource = configs.First();

            Assert.Equal("example", imageResource.Name);
            Assert.True(imageResource.Ignore);
            Assert.Equal(1, imageResource.Scale);
        }

        [Fact]
        public void PlatformIconDoesNotUseWatermark()
        {
            var config = GetConfiguration();
            var generator = CreateGenerator(config, ImageDirectory, PlatformImageDirectory);
            //generator.Execute();

            var resource = generator.GetResourceDefinition(PlatformIcon);
            var configs = resource.GetConfigurations(Platform);
            Assert.Single(configs);
            var imageResource = configs.First();

            Assert.NotEqual(Path.GetFileNameWithoutExtension(PlatformIcon), imageResource.Name);
            Assert.False(imageResource.Ignore);
            Assert.Equal(1, imageResource.Scale);
            Assert.Null(imageResource.Watermark);
        }

        [Fact]
        public void PlatformIconUsesWatermarkForDebug()
        {
            var config = GetConfiguration();
            var generator = CreateGenerator(config, ImageDirectory, PlatformImageDirectory, DebugImageDirectory);
            //generator.Execute();

            var resource = generator.GetResourceDefinition(PlatformIcon);
            var configs = resource.GetConfigurations(Platform);
            Assert.Single(configs);
            var imageResource = configs.First();

            Assert.NotEqual(Path.GetFileNameWithoutExtension(PlatformIcon), imageResource.Name);
            Assert.False(imageResource.Ignore);
            Assert.Equal(1, imageResource.Scale);
            Assert.Equal("example", imageResource.Watermark?.SourceFile);
        }

        [Theory]
        [InlineData("dotnetbot")]
        [InlineData("platform")]
        public void StandardImageGeneratesProperOutputs(string file)
        {
            var config = GetConfiguration();
            var generator = CreateGenerator(config, ImageDirectory, PlatformImageDirectory);
            generator.Execute();

            var imagePath = file == "platform" ? PlatformIcon : Path.Combine(ImageDirectory, $"{file}.png");
            var resource = generator.GetResourceDefinition(imagePath);
            var expectedOutputs = ExpectedOutputs[file];
            var resourceConfig = resource.GetConfigurations(Platform).First();
            if(Platform == Platform.Android)
            {
                Assert.Equal(file == "platform" ? PlatformResourceType.Mipmap : PlatformResourceType.Drawable, resourceConfig.ResourceType);
            }
            var actualOutputs = generator.GetOutputImages(resourceConfig);

            Assert.Equal(expectedOutputs.Count(), actualOutputs.Count());

            foreach(var expectedOutput in expectedOutputs)
            {
                var expectedOutputPath = Path.Combine(config.IntermediateOutputPath, expectedOutput.OutputFile);
                var actualOutput = actualOutputs.FirstOrDefault(x => x.OutputFile == expectedOutputPath);
                Assert.NotNull(actualOutput);
                Assert.Equal(expectedOutput.Height, actualOutput.Height);
                Assert.Equal(expectedOutput.Width, actualOutput.Width);
                Assert.Equal(expectedOutput.InputFile, actualOutput.InputFile);
                Assert.Equal(expectedOutput.OutputLink, actualOutput.OutputLink);
                Assert.Equal(expectedOutput.RequiresBackgroundColor, actualOutput.RequiresBackgroundColor);
                Assert.Equal(expectedOutput.Scale, actualOutput.Scale);
                Assert.Equal(expectedOutput.ShouldBeVisible, actualOutput.ShouldBeVisible);
                Assert.Equal(expectedOutput.Watermark?.SourceFile, actualOutput.Watermark?.SourceFile);
            }
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
