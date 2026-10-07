using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Tasks;
using Mobile.BuildTools.Tests.Mocks;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Tasks
{
    [Collection("MSBuild")]
    public sealed class LottieBuildTargetsFixture : IDisposable
    {
        private readonly string directory = Path.Combine(Path.GetTempPath(), "LottieTargets-" + Guid.NewGuid());
        public LottieBuildTargetsFixture()
        {
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, "buildtools.json"), "{}");
            File.WriteAllText(Path.Combine(directory, "logo.json"), "{ \"layers\": [] }");
        }
        public void Dispose() => Directory.Delete(directory, true);

        [Theory]
        [InlineData("android", true, "MauiAsset", "GetMauiItems")]
        [InlineData("ios", true, "MauiAsset", "ProcessMauiAssets")]
        [InlineData("android", false, "AndroidAsset", "_ComputeAndroidAssets")]
        [InlineData("ios", false, "BundleResource", "_CollectBundleResources")]
        [InlineData("maccatalyst", false, "BundleResource", "AssignTargetPaths")]
        [InlineData("windows", false, "Content", "AssignTargetPaths")]
        [InlineData("browserwasm", false, "Content", "GetCopyToOutputDirectoryItems")]
        public void RawAdaptersRetainNamesAndNoOpRegistration(string platform, bool maui, string kind, string consumer)
        {
            var project = CreateProject(platform, maui, kind, consumer);
            var first = Build(project, consumer);
            var item = Assert.Single(first.GetItems("Observed"));
            Assert.Equal("animations/logo.json", item.GetMetadataValue("LogicalName"));
            Assert.Equal("animations/logo.json", item.GetMetadataValue("Link"));
            Assert.Equal("preserved", item.GetMetadataValue("Custom"));
            Assert.Empty(first.GetItems("MauiImage"));
            Assert.Empty(first.GetItems("UnifiedImageAsset"));
            Assert.Contains(first.GetItems("FileWrites"), x => x.EvaluatedInclude == item.EvaluatedInclude);
            var time = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            File.SetLastWriteTimeUtc(item.EvaluatedInclude, time);
            var repeated = Assert.Single(Build(project, consumer).GetItems("Observed"));
            Assert.Equal(item.EvaluatedInclude, repeated.EvaluatedInclude);
            Assert.Equal(time, File.GetLastWriteTimeUtc(repeated.EvaluatedInclude));
            File.WriteAllText(Path.Combine(directory, "buildtools.json"), "{\"images\":{\"optimizeLottie\":false}}");
            var raw = Assert.Single(Build(project, consumer).GetItems("Observed"));
            Assert.Equal("{ \"layers\": [] }", File.ReadAllText(raw.EvaluatedInclude));
            File.WriteAllText(Path.Combine(directory, "buildtools.json"), "{}");
            Assert.Equal("{\"layers\":[]}", File.ReadAllText(Assert.Single(Build(project, consumer).GetItems("Observed")).EvaluatedInclude));
        }

        [Fact]
        public void ConditionalAndExplicitPathsFollowImagesPrecedence()
        {
            foreach (var folder in new[] { "shared", "android", "brand", "first", "second" })
            {
                Directory.CreateDirectory(Path.Combine(directory, folder));
                File.WriteAllText(Path.Combine(directory, folder, "logo.json"), "{\"nm\":\"" + folder + "\"}");
            }
            File.WriteAllText(Path.Combine(directory, "buildtools.json"), "{\"images\":{\"directories\":[\"shared\"],\"conditionalDirectories\":{\"android\":[\"android\"],\"Debug\":[\"brand\"]}}}");
            var project = CreateProject("android", false, "AndroidAsset", "AssignTargetPaths");
            string Read(Dictionary<string, string> properties = null) => File.ReadAllText(Assert.Single(Build(project, "AssignTargetPaths", properties).GetItems("Observed")).EvaluatedInclude);
            Assert.Contains("brand", Read());
            Assert.Contains("first", Read(new Dictionary<string, string> { ["BuildToolsImageSearchPath"] = "first;second" }));
            File.Delete(Path.Combine(directory, "first", "logo.json"));
            File.Delete(Path.Combine(directory, "second", "logo.json"));
            Assert.Equal("{\"layers\":[]}", Read(new Dictionary<string, string> { ["BuildToolsImageSearchPath"] = "first;second", ["BuildToolsIgnoreDefaultSearchPath"] = "true" }));
        }

        [Theory]
        [InlineData("android", "AndroidAsset", "_ResizetizerIsAndroidApp")]
        [InlineData("ios", "Content", "_ResizetizerIsiOSApp")]
        [InlineData("windows", "ContentWithTargetPath", "_ResizetizerIsWindowsAppSdk")]
        public void RealMauiAssetTargetRetainsLogicalPaths(string platform, string kind, string flag)
        {
            var package = GetType().Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().Single(x => x.Key == "MauiResizetizerPackagePath").Value;
            var realTargets = XDocument.Load(Path.Combine(package, "buildTransitive/Microsoft.Maui.Resizetizer.After.targets"));
            var target = realTargets.Root.Elements().Where(x => x.Name.LocalName == "Target").Single(x => (string)x.Attribute("Name") == "ProcessMauiAssets");
            var project = CreateProject(platform, true, "MauiAsset", "Observe");
            var xml = XDocument.Load(project);
            xml.Root.Element("PropertyGroup").Add(new XElement(flag, "True"), new XElement("EnableMauiAssetProcessing", "true"));
            xml.Root.AddFirst(new XElement("UsingTask", new XAttribute("TaskName", "Microsoft.Maui.Resizetizer.GetMauiAssetPath"),
                new XAttribute("AssemblyFile", Path.Combine(package, "buildTransitive/Microsoft.Maui.Resizetizer.dll"))));
            var imported = new XElement(target);
            foreach (var element in imported.DescendantsAndSelf()) element.Name = element.Name.LocalName;
            xml.Root.Add(imported);
            var observe = xml.Root.Elements("Target").Single(x => (string)x.Attribute("Name") == "Observe");
            observe.SetAttributeValue("DependsOnTargets", "ProcessMauiAssets");
            observe.Element("ItemGroup").Element("Observed").SetAttributeValue("Include", "@(" + kind + ")");
            xml.Save(project);
            var item = Assert.Single(Build(project, "Observe").GetItems("Observed"));
            Assert.True(File.Exists(item.EvaluatedInclude));
            Assert.Equal("animations/logo.json", item.GetMetadataValue(platform == "windows" ? "TargetPath" : "Link"));
        }

        [Fact]
        public void ItemRemovalRetiresGeneratedOutputsAndDisableKeepsOriginal()
        {
            var project = CreateProject("browserwasm", false, "Content", "AssignTargetPaths");
            var first = Assert.Single(Build(project, "AssignTargetPaths").GetItems("Observed")).EvaluatedInclude;
            var disabled = Build(project, "AssignTargetPaths", new Dictionary<string, string> { ["BuildToolsEnableImageProcessing"] = "false" });
            Assert.Equal(Path.Combine(directory, "logo.json"), Path.GetFullPath(Assert.Single(disabled.GetItems("Observed")).EvaluatedInclude, directory));
            var xml = XDocument.Load(project);
            xml.Root.Element("ItemGroup").Element("MobileBuildToolsLottie").Remove();
            xml.Save(project);
            Build(project, "AssignTargetPaths");
            Assert.False(File.Exists(first));
            Assert.True(File.Exists(Path.Combine(directory, "logo.json")));
        }

        private string CreateProject(string platform, bool maui, string kind, string consumer)
        {
            var repository = new DirectoryInfo(AppContext.BaseDirectory);
            const string relative = "src/Mobile.BuildTools.Images/Mobile.BuildTools.Images.targets";
            while (!File.Exists(Path.Combine(repository.FullName, relative))) repository = repository.Parent ?? throw new InvalidOperationException();
            var project = Path.Combine(directory, "Lottie.proj");
            File.WriteAllText(project, $"""
                <Project>
                  <PropertyGroup>
                    <TargetFramework>net10.0-{platform}</TargetFramework>
                    <TargetPlatformIdentifier>{platform}</TargetPlatformIdentifier>
                    <UseMaui>{maui.ToString().ToLowerInvariant()}</UseMaui>
                    <Configuration>Debug</Configuration>
                    <SolutionDir>{Escape(directory)}</SolutionDir>
                    <IntermediateOutputPath>obj/</IntermediateOutputPath>
                    <BuildToolsConfigFilePath>{Escape(Path.Combine(directory, "buildtools.json"))}</BuildToolsConfigFilePath>
                  </PropertyGroup>
                  <ItemGroup>
                    <MobileBuildToolsLottie Include="logo.json" LogicalName="animations/logo.json" Custom="preserved" />
                    <{kind} Include="logo.json" />
                  </ItemGroup>
                  <UsingTask TaskName="Mobile.BuildTools.Tasks.CollectImageAssetsTask" AssemblyFile="{Escape(typeof(CollectImageAssetsTask).Assembly.Location)}" />
                  <UsingTask TaskName="Mobile.BuildTools.Tasks.ImageResizerTask" AssemblyFile="{Escape(typeof(ImageResizerTask).Assembly.Location)}" />
                  <UsingTask TaskName="Mobile.BuildTools.Tasks.PrepareLottieAssetsTask" AssemblyFile="{Escape(typeof(PrepareLottieAssetsTask).Assembly.Location)}" />
                  <Target Name="MobileBuildToolsInit"><PropertyGroup><BuildToolsEnableImageProcessing Condition="'$(BuildToolsEnableImageProcessing)' == ''">true</BuildToolsEnableImageProcessing></PropertyGroup></Target>
                  <Import Project="{Escape(Path.Combine(repository.FullName, relative))}" />
                  <Target Name="{consumer}"><ItemGroup><Observed Include="@({kind})" /></ItemGroup></Target>
                </Project>
                """);
            return project;
        }
        private static ProjectInstance Build(string project, string target, Dictionary<string, string> properties = null)
        {
            using var collection = new ProjectCollection();
            var logger = new RecordingBuildLogger();
            var instance = new ProjectInstance(project, properties ?? new Dictionary<string, string>(), null, collection);
            using var manager = new BuildManager();
            var result = manager.Build(new BuildParameters(collection) { MaxNodeCount = 1, EnableNodeReuse = false, Loggers = new ILogger[] { logger } },
                new BuildRequestData(instance, new[] { target }, null, BuildRequestDataFlags.ProvideProjectStateAfterBuild));
            Assert.True(result.OverallResult == BuildResultCode.Success, string.Join(Environment.NewLine, logger.Errors));
            return result.ProjectStateAfterBuild;
        }
        private static string Escape(string value) => SecurityElement.Escape(value);
    }
}
