using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Tasks;
using Mobile.BuildTools.Tests.Mocks;
using SkiaSharp;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Tasks
{
    public sealed class NativeImageBuildTargetsFixture : IDisposable
    {
        private readonly string directory = Path.Combine(Path.GetTempPath(), nameof(NativeImageBuildTargetsFixture), Guid.NewGuid().ToString("N"));

        public NativeImageBuildTargetsFixture()
        {
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, "buildtools.json"), "{}");
            using var bitmap = new SKBitmap(16, 16);
            bitmap.Erase(SKColors.Red);
            using var stream = File.Create(Path.Combine(directory, "logo.png"));
            bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
        }

        [Theory]
        [InlineData("MauiImage", "GetMauiItems", false)]
        [InlineData("MauiImage", "ResizetizeCollectItems", false)]
        [InlineData("MauiIcon", "ResizetizeCollectItems", false)]
        [InlineData("MauiSplashScreen", "ResizetizeCollectItems", false)]
        [InlineData("UnoImage", "GetUnoItems", false)]
        [InlineData("UnoImage", "UnoResizetizeCollectItems", false)]
        [InlineData("UnoIcon", "UnoResizetizeCollectItems", false)]
        [InlineData("UnoSplashScreen", "UnoResizetizeCollectItems", false)]
        [InlineData("UnoSplashScreen", "GenerateUnoSplashAndroid", true)]
        public void PreparesBeforeNativeConsumersAndRetainsMetadata(string kind, string consumer, bool designTime)
        {
            var project = CreateProject(kind, consumer, designTime);
            var state = Build(project, consumer);
            var observed = Assert.Single(state.GetItems("ObservedImage"));
            Assert.NotEqual("logo.png", observed.EvaluatedInclude);
            Assert.True(File.Exists(observed.EvaluatedInclude));
            Assert.Equal("preserved", observed.GetMetadataValue("Custom"));
            Assert.Equal("24,24", observed.GetMetadataValue("BaseSize"));
            Assert.Equal("Assets/logo.png", observed.GetMetadataValue("Link"));
            Assert.Equal("false", observed.GetMetadataValue("Resize"));
            Assert.Empty(state.GetItems("UnifiedImageAsset"));
            Assert.Equal(2, state.GetItems("FileWrites").Count);
            Assert.False(File.Exists(Path.Combine(directory, "logo.json")));
        }

        [Fact]
        public void NoOpAndRuntimeIsolationSurviveSeparateBuilds()
        {
            var project = CreateProject("UnoImage", "UnoResizetizeCollectItems", false);
            var first = Assert.Single(Build(project, "UnoResizetizeCollectItems").GetItems("ObservedImage")).EvaluatedInclude;
            var historical = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            File.SetLastWriteTimeUtc(first, historical);
            var repeated = Assert.Single(Build(project, "UnoResizetizeCollectItems").GetItems("ObservedImage")).EvaluatedInclude;
            Assert.Equal(first, repeated);
            Assert.Equal(historical, File.GetLastWriteTimeUtc(repeated));
            var other = Assert.Single(Build(project, "UnoResizetizeCollectItems", "android-x64").GetItems("ObservedImage")).EvaluatedInclude;
            Assert.NotEqual(first, other);
        }

        private string CreateProject(string kind, string consumer, bool designTime)
        {
            var repository = new DirectoryInfo(AppContext.BaseDirectory);
            const string relativeTargets = "src/Mobile.BuildTools.Images/Mobile.BuildTools.Images.targets";
            while (!File.Exists(Path.Combine(repository.FullName, relativeTargets)))
                repository = repository.Parent ?? throw new InvalidOperationException("Repository image targets not found.");
            var project = Path.Combine(directory, "NativeImages.proj");
            File.WriteAllText(project, $"""
                <Project>
                  <PropertyGroup>
                    <TargetFramework>net10.0-android36.0</TargetFramework>
                    <TargetPlatformIdentifier>android</TargetPlatformIdentifier>
                    <Configuration>Debug</Configuration>
                    <SolutionDir>{Escape(directory)}</SolutionDir>
                    <IntermediateOutputPath>obj/</IntermediateOutputPath>
                    <BuildToolsConfigFilePath>{Escape(Path.Combine(directory, "buildtools.json"))}</BuildToolsConfigFilePath>
                    <DesignTimeBuild>{designTime}</DesignTimeBuild>
                  </PropertyGroup>
                  <ItemGroup>
                    <{kind} Include="logo.png" Link="Assets/logo.png" BaseSize="24,24" Resize="false" Custom="preserved" />
                  </ItemGroup>
                  <UsingTask TaskName="Mobile.BuildTools.Tasks.PrepareNativeImageAssetsTask" AssemblyFile="{Escape(typeof(PrepareNativeImageAssetsTask).Assembly.Location)}" />
                  <Target Name="MobileBuildToolsInit"><PropertyGroup><BuildToolsEnableImageProcessing>true</BuildToolsEnableImageProcessing></PropertyGroup></Target>
                  <Import Project="{Escape(Path.Combine(repository.FullName, relativeTargets))}" />
                  <Target Name="{consumer}"><ItemGroup><ObservedImage Include="@({kind})" /></ItemGroup></Target>
                </Project>
                """);
            return project;
        }

        private static ProjectInstance Build(string project, string target, string runtime = "android-arm64")
        {
            using var collection = new ProjectCollection();
            var logger = new RecordingBuildLogger();
            var instance = new ProjectInstance(project, new Dictionary<string, string> { ["RuntimeIdentifier"] = runtime }, null, collection);
            using var manager = new BuildManager();
            var result = manager.Build(new BuildParameters(collection) { MaxNodeCount = 1, EnableNodeReuse = false, Loggers = new ILogger[] { logger } },
                new BuildRequestData(instance, new[] { target }, null, BuildRequestDataFlags.ProvideProjectStateAfterBuild));
            Assert.True(result.OverallResult == BuildResultCode.Success, string.Join(Environment.NewLine, logger.Errors));
            return result.ProjectStateAfterBuild;
        }

        private static string Escape(string value) => SecurityElement.Escape(value);
        public void Dispose() => Directory.Delete(directory, true);
    }
}
