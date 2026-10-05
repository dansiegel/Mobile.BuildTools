using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Xml.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Tasks;
using Mobile.BuildTools.Tests.Mocks;
using SkiaSharp;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Tasks
{
    [Collection("MSBuild")]
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

        [Theory]
        [InlineData("MauiIcon")]
        [InlineData("UnoIcon")]
        public void ImagesPackageAloneSuppliesAndroidReferencesBeforeFirstManifestRead(string kind)
        {
            var manifest = Path.Combine(directory, "AndroidManifest.xml");
            const string original = "<manifest package=\"com.example\"><application /></manifest>";
            File.WriteAllText(manifest, original);
            var project = CreateProject(kind, "_GetAndroidPackageName", false);
            var xml = XDocument.Load(project);
            xml.Root.Element("PropertyGroup").Add(new XElement("AndroidApplication", "true"), new XElement("AndroidManifest", manifest));
            xml.Root.AddFirst(new XElement("UsingTask", new XAttribute("TaskName", "Mobile.BuildTools.Tasks.GenerateNativeImageManifestTask"),
                new XAttribute("AssemblyFile", typeof(GenerateNativeImageManifestTask).Assembly.Location)));
            xml.Save(project);

            var state = Build(project, "_GetAndroidPackageName");

            var output = Path.Combine(directory, state.GetPropertyValue("AndroidManifest"));
            XNamespace android = "http://schemas.android.com/apk/res/android";
            var application = XDocument.Load(output).Root.Element("application");
            Assert.Equal("@mipmap/logo", (string)application.Attribute(android + "icon"));
            Assert.Equal("@mipmap/logo_round", (string)application.Attribute(android + "roundIcon"));
            Assert.NotEqual(manifest, output);
            Assert.Equal(original, File.ReadAllText(manifest));
            Assert.Contains(state.GetItems("FileWrites"), item => item.EvaluatedInclude == Path.GetFullPath(output));
            Assert.Empty(state.GetItems("PartialAppManifest"));
        }

        [Fact]
        public void OptionalManifestTransformRunsBeforeImageReferenceCompletion()
        {
            var manifest = Path.Combine(directory, "AndroidManifest.xml");
            var transformed = Path.Combine(directory, "Transformed.xml");
            File.WriteAllText(manifest, "<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\"><application android:icon=\"$Icon$\" /></manifest>");
            File.WriteAllText(transformed, "<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\"><application android:icon=\"@mipmap/logo\" /></manifest>");
            var project = CreateProject("MauiIcon", "_GetAndroidPackageName", false);
            var xml = XDocument.Load(project);
            xml.Root.Element("PropertyGroup").Add(new XElement("AndroidApplication", "true"), new XElement("AndroidManifest", manifest),
                new XElement("MobileBuildToolsAndroidImageManifestDependsOn", "TransformManifest"));
            xml.Root.AddFirst(new XElement("UsingTask", new XAttribute("TaskName", "Mobile.BuildTools.Tasks.GenerateNativeImageManifestTask"),
                new XAttribute("AssemblyFile", typeof(GenerateNativeImageManifestTask).Assembly.Location)));
            xml.Root.Add(new XElement("Target", new XAttribute("Name", "TransformManifest"),
                new XElement("PropertyGroup", new XElement("AndroidManifest", transformed))));
            xml.Save(project);

            var state = Build(project, "_GetAndroidPackageName");

            var output = Path.Combine(directory, state.GetPropertyValue("AndroidManifest"));
            XNamespace android = "http://schemas.android.com/apk/res/android";
            var application = XDocument.Load(output).Root.Element("application");
            Assert.Equal("@mipmap/logo", (string)application.Attribute(android + "icon"));
            Assert.Equal("@mipmap/logo_round", (string)application.Attribute(android + "roundIcon"));
            Assert.Contains("$Icon$", File.ReadAllText(manifest));
        }

        [Theory]
        [InlineData("Maui", "ResizetizeCollectItems")]
        [InlineData("Uno", "UnoResizetizeCollectItems")]
        public void AndroidLinksIconImportedByNativeCollection(string framework, string collection)
        {
            var manifest = Path.Combine(directory, "AndroidManifest.xml");
            File.WriteAllText(manifest, "<manifest><application /></manifest>");
            var project = CreateProject(framework + "Image", collection, false);
            var xml = XDocument.Load(project);
            xml.Root.Element("PropertyGroup").Add(new XElement("AndroidApplication", "true"), new XElement("AndroidManifest", manifest));
            xml.Root.AddFirst(new XElement("UsingTask", new XAttribute("TaskName", "Mobile.BuildTools.Tasks.GenerateNativeImageManifestTask"),
                new XAttribute("AssemblyFile", typeof(GenerateNativeImageManifestTask).Assembly.Location)));
            xml.Root.Add(new XElement("Target", new XAttribute("Name", "PrepareForBuild")));
            xml.Root.Elements("Target").Single(target => (string)target.Attribute("Name") == collection).Add(
                new XElement("ItemGroup", new XElement(framework + "Icon", new XAttribute("Include", "logo.png"), new XAttribute("Link", "Assets/sharedicon.png"))));
            xml.Save(project);

            var state = Build(project, "PrepareForBuild;" + collection);

            var output = Path.Combine(directory, state.GetPropertyValue("AndroidManifest"));
            XNamespace android = "http://schemas.android.com/apk/res/android";
            var application = XDocument.Load(output).Root.Element("application");
            Assert.Equal("@mipmap/sharedicon", (string)application.Attribute(android + "icon"));
            Assert.Equal("@mipmap/sharedicon_round", (string)application.Attribute(android + "roundIcon"));
        }

        [Theory]
        [InlineData("MauiIcon", "DisableResizetizer", "true")]
        [InlineData("MauiIcon", "EnableMauiImageProcessing", "false")]
        [InlineData("UnoIcon", "DisableUnoResizetizer", "true")]
        public void DisabledNativePipelineDoesNotCreateManifestReferences(string kind, string property, string value)
        {
            var manifest = Path.Combine(directory, "AndroidManifest.xml");
            File.WriteAllText(manifest, "<manifest><application /></manifest>");
            var project = CreateProject(kind, "_GetAndroidPackageName", false);
            var xml = XDocument.Load(project);
            xml.Root.Element("PropertyGroup").Add(new XElement("AndroidApplication", "true"), new XElement("AndroidManifest", manifest), new XElement(property, value));
            xml.Root.AddFirst(new XElement("UsingTask", new XAttribute("TaskName", "Mobile.BuildTools.Tasks.GenerateNativeImageManifestTask"),
                new XAttribute("AssemblyFile", typeof(GenerateNativeImageManifestTask).Assembly.Location)));
            xml.Save(project);

            var state = Build(project, "_GetAndroidPackageName");

            Assert.Equal(manifest, state.GetPropertyValue("AndroidManifest"));
        }

        [Theory]
        [InlineData("MauiIcon")]
        [InlineData("UnoIcon")]
        public void ImagesPackageAloneAddsAppleSelectorAndKeepsFrameworkSplashPartial(string kind)
        {
            var manifest = Path.Combine(directory, "Info.plist");
            var splash = Path.Combine(directory, "SplashInfo.plist");
            File.WriteAllText(manifest, "<plist><dict /></plist>");
            File.WriteAllText(splash, "<plist><dict><key>UILaunchStoryboardName</key><string>FrameworkSplash</string></dict></plist>");
            var project = CreateProject(kind, "CollectAppManifests", false);
            var xml = XDocument.Load(project);
            var properties = xml.Root.Element("PropertyGroup");
            properties.Element("TargetFramework").Value = "net10.0-ios26.0";
            properties.Element("TargetPlatformIdentifier").Value = "ios";
            properties.Add(new XElement("OutputType", "Exe"), new XElement("AppBundleManifest", manifest));
            xml.Root.AddFirst(new XElement("UsingTask", new XAttribute("TaskName", "Mobile.BuildTools.Tasks.GenerateNativeImageManifestTask"),
                new XAttribute("AssemblyFile", typeof(GenerateNativeImageManifestTask).Assembly.Location)));
            xml.Root.Add(new XElement("Target", new XAttribute("Name", "_DetectAppManifest")));
            xml.Root.Elements("Target").Single(target => (string)target.Attribute("Name") == "CollectAppManifests")
                .SetAttributeValue("DependsOnTargets", "$(CollectAppManifestsDependsOn)");
            xml.Root.Add(new XElement("ItemGroup", new XElement("PartialAppManifest", new XAttribute("Include", splash), new XAttribute("Custom", "framework"))));
            xml.Save(project);

            var state = Build(project, "CollectAppManifests", "iossimulator-arm64");

            Assert.Equal(manifest, state.GetPropertyValue("AppBundleManifest"));
            var partials = state.GetItems("PartialAppManifest").ToArray();
            Assert.Equal(2, partials.Length);
            Assert.Equal("framework", partials.Single(item => item.EvaluatedInclude == splash).GetMetadataValue("Custom"));
            var generated = partials.Single(item => item.EvaluatedInclude != splash);
            Assert.Equal("false", generated.GetMetadataValue("Overwrite"));
            var dictionary = XDocument.Load(generated.EvaluatedInclude).Root.Element("dict");
            Assert.Equal("XSAppIconAssets", Assert.Single(dictionary.Elements("key")).Value);
            Assert.Equal("Assets.xcassets/logo.appiconset", Assert.Single(dictionary.Elements("string")).Value);
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
                new BuildRequestData(instance, target.Split(';'), null, BuildRequestDataFlags.ProvideProjectStateAfterBuild));
            Assert.True(result.OverallResult == BuildResultCode.Success, string.Join(Environment.NewLine, logger.Errors));
            return result.ProjectStateAfterBuild;
        }

        private static string Escape(string value) => SecurityElement.Escape(value);
        public void Dispose() => Directory.Delete(directory, true);
    }
}
