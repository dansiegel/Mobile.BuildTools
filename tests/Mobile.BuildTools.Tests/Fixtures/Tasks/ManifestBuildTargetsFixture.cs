using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Tasks;
using Mobile.BuildTools.Tests.Mocks;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Tasks;

[Collection("MSBuild")]
public sealed class ManifestBuildTargetsFixture : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), nameof(ManifestBuildTargetsFixture), Guid.NewGuid().ToString("N"));

    public ManifestBuildTargetsFixture()
    {
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "buildtools.json"), """
            { "manifests": { "token": "$", "missingTokensAsErrors": true },
              "automaticVersioning": { "behavior": "Off" },
              "environment": { "defaults": { "Manifest_Value": "A & B", "Manifest_Package": "com.example.test" } } }
            """);
    }

    [Fact]
    public void AndroidTransformsBeforeFirstPackageReadAndTracksOutputs()
    {
        File.WriteAllText(Path.Combine(directory, "AndroidManifest.xml"), "<manifest package=\"$Package$\"><application label=\"$Value$\"/></manifest>");
        var project = CreateProject("AndroidManifest.targets", "net10.0-android36.0", "<AndroidManifest>AndroidManifest.xml</AndroidManifest>", "", """
            <Target Name="PrepareForBuild" />
            <Target Name="_GetAndroidPackageName" DependsOnTargets="PrepareForBuild">
              <PropertyGroup><ObservedManifest>$(MSBuildProjectDirectory)/$(AndroidManifest)</ObservedManifest></PropertyGroup>
            </Target>
            """);
        var state = Build(project, "_GetAndroidPackageName");
        var manifest = state.GetPropertyValue("ObservedManifest");
        Assert.Equal("com.example.test", XDocument.Load(manifest).Root.Attribute("package").Value);
        Assert.Equal("A & B", XDocument.Load(manifest).Root.Element("application").Attribute("label").Value);
        Assert.Contains(state.GetItems("FileWrites"), item => Path.GetFullPath(item.EvaluatedInclude) == Path.GetFullPath(manifest));
        Assert.Contains("$Package$", File.ReadAllText(Path.Combine(directory, "AndroidManifest.xml")));
    }

    [Fact]
    public void ApplePreservesUserAndLateFrameworkPartialsWithMetadata()
    {
        File.WriteAllText(Path.Combine(directory, "Info.plist"), Plist("Name", "$Value$"));
        File.WriteAllText(Path.Combine(directory, "User.plist"), Plist("User", "$Value$"));
        File.WriteAllText(Path.Combine(directory, "Framework.plist"), Plist("UILaunchStoryboardName", "FrameworkSplash"));
        var project = CreateProject("AppleManifests.targets", "net10.0-ios26.0", """
            <AppBundleManifest>Info.plist</AppBundleManifest>
            <CollectAppManifestsDependsOn>InjectFrameworkPartial</CollectAppManifestsDependsOn>
            """, "<PartialAppManifest Include=\"User.plist\" Overwrite=\"false\" Custom=\"preserved\" />", """
            <Target Name="_DetectAppManifest" />
            <Target Name="InjectFrameworkPartial"><ItemGroup><PartialAppManifest Include="Framework.plist" /></ItemGroup></Target>
            <Target Name="CollectAppManifests" DependsOnTargets="$(CollectAppManifestsDependsOn)" />
            """);
        var state = Build(project, "CollectAppManifests");
        var partials = state.GetItems("PartialAppManifest").ToArray();
        Assert.Equal(2, partials.Length);
        Assert.Equal("false", partials[0].GetMetadataValue("Overwrite"));
        Assert.Equal("preserved", partials[0].GetMetadataValue("Custom"));
        Assert.Equal("A & B", XDocument.Load(partials[0].EvaluatedInclude).Descendants("string").Single().Value);
        Assert.Equal("FrameworkSplash", XDocument.Load(partials[1].EvaluatedInclude).Descendants("string").Single().Value);
        Assert.Equal("A & B", XDocument.Load(state.GetPropertyValue("AppBundleManifest")).Descendants("string").Single().Value);
        Assert.Contains("$Value$", File.ReadAllText(Path.Combine(directory, "User.plist")));
    }

    [Fact]
    public void AppleRetainsOptInProductionPushEntitlement()
    {
        var project = CreateProject("AppleManifests.targets", "net10.0-ios26.0", "<APSProductionEnvironment>true</APSProductionEnvironment>",
            "<CustomEntitlements Include=\"aps-environment\" Type=\"String\" Value=\"development\" /><CustomEntitlements Include=\"unrelated\" Type=\"Boolean\" Value=\"true\" />", "<Target Name=\"_CompileEntitlements\" />");
        var entitlements = Build(project, "_CompileEntitlements").GetItems("CustomEntitlements");
        Assert.Equal(2, entitlements.Count);
        Assert.Equal("production", entitlements.Single(item => item.EvaluatedInclude == "aps-environment").GetMetadataValue("Value"));
        Assert.Equal("true", entitlements.Single(item => item.EvaluatedInclude == "unrelated").GetMetadataValue("Value"));
    }

    [Theory]
    [InlineData("AppxManifest", "MauiGeneratePackageAppxManifest")]
    [InlineData("_MauiAppxManifest", "MauiGeneratePackageAppxManifest")]
    [InlineData("_UnoAppxManifest", "UnoGeneratePackageAppxManifest")]
    [InlineData("_SkiaManifest", "UnoGeneratePackageAppxManifest")]
    public void WindowsTransformsInputsBeforeFrameworkTransforms(string itemType, string consumer)
    {
        File.WriteAllText(Path.Combine(directory, "Package.appxmanifest"), "<Package><Properties><DisplayName>$Value$</DisplayName></Properties></Package>");
        var project = CreateProject("WindowsManifest.targets", "net10.0-windows10.0.19041.0", "", $"<{itemType} Include=\"Package.appxmanifest\" Link=\"Package.appxmanifest\" Custom=\"preserved\" />", $"<Target Name=\"{consumer}\" />");
        var state = Build(project, consumer);
        var item = Assert.Single(state.GetItems(itemType));
        Assert.Equal("A & B", XDocument.Load(item.EvaluatedInclude).Descendants("DisplayName").Single().Value);
        Assert.Equal("preserved", item.GetMetadataValue("Custom"));
        Assert.Equal("Package.appxmanifest", item.GetMetadataValue("Link"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void WebAssemblyTransformsPwaJsonWithoutTouchingBootJavaScript(bool absolutePath)
    {
        File.WriteAllText(Path.Combine(directory, "manifest.webmanifest"), "{\"name\":\"$Value$\"}");
        File.WriteAllText(Path.Combine(directory, "AppManifest.js"), "window.appManifest = { displayName: '$Value$' };");
        var source = absolutePath ? Path.Combine(directory, "manifest.webmanifest") : "manifest.webmanifest";
        var project = CreateProject("WebAssemblyManifest.targets", "net10.0-browser", $"<WasmPWAManifestFile>{Escape(source)}</WasmPWAManifestFile>", $"<Content Include=\"{Escape(source)}\" />", "<Target Name=\"UnoResizetizeImages\" />");
        var state = Build(project, "UnoResizetizeImages");
        using var json = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory, state.GetPropertyValue("WasmPWAManifestFile"))));
        Assert.Equal("A & B", json.RootElement.GetProperty("name").GetString());
        Assert.Contains("$Value$", File.ReadAllText(Path.Combine(directory, "AppManifest.js")));
        var content = Assert.Single(state.GetItems("Content"));
        Assert.Equal("manifest.webmanifest", content.GetMetadataValue("Link"));
        Assert.Equal("manifest.webmanifest", content.GetMetadataValue("TargetPath"));
    }

    [Fact]
    public void RepeatedBuildIsNoOpAndTargetRuntimeOutputsAreIsolated()
    {
        File.WriteAllText(Path.Combine(directory, "AndroidManifest.xml"), "<manifest package=\"$Package$\" />");
        var project = CreateProject("AndroidManifest.targets", "net10.0-android36.0", "<AndroidManifest>AndroidManifest.xml</AndroidManifest>", "", "<Target Name=\"PrepareForBuild\" />");
        var first = Build(project, "PrepareForBuild", "android-arm64");
        var path = Path.Combine(directory, first.GetPropertyValue("AndroidManifest"));
        var timestamp = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        File.SetLastWriteTimeUtc(path, timestamp);
        var repeated = Build(project, "PrepareForBuild", "android-arm64");
        Assert.Equal(timestamp, File.GetLastWriteTimeUtc(Path.Combine(directory, repeated.GetPropertyValue("AndroidManifest"))));
        var other = Build(project, "PrepareForBuild", "android-x64");
        Assert.NotEqual(repeated.GetPropertyValue("AndroidManifest"), other.GetPropertyValue("AndroidManifest"));
        File.Delete(path);
        Build(project, "PrepareForBuild", "android-arm64");
        Assert.True(File.Exists(path));
    }

    private string CreateProject(string targets, string framework, string properties, string items, string targetDefinitions)
    {
        var repository = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(repository.FullName, "src", "Mobile.BuildTools.AppManifests", targets)))
            repository = repository.Parent ?? throw new InvalidOperationException("Repository targets not found.");
        var import = Path.Combine(repository.FullName, "src", "Mobile.BuildTools.AppManifests", targets);
        var taskAssembly = typeof(TransformManifestItemsTask).Assembly.Location;
        var project = Path.Combine(directory, "ManifestTargets.proj");
        File.WriteAllText(project, $"""
            <Project>
              <PropertyGroup>
                <TargetFramework>{framework}</TargetFramework>
                <Configuration>Debug</Configuration>
                <SolutionDir>{Escape(directory)}</SolutionDir>
                <IntermediateOutputPath>obj/</IntermediateOutputPath>
                <BuildToolsConfigFilePath>{Escape(Path.Combine(directory, "buildtools.json"))}</BuildToolsConfigFilePath>
                {properties}
              </PropertyGroup>
              <ItemGroup>{items}</ItemGroup>
              <UsingTask TaskName="Mobile.BuildTools.Tasks.TransformManifestItemsTask" AssemblyFile="{Escape(taskAssembly)}" />
              <UsingTask TaskName="AssignTargetPath" AssemblyFile="{Escape(typeof(Microsoft.Build.Tasks.AssignTargetPath).Assembly.Location)}" />
              <Target Name="MobileBuildToolsInit"><PropertyGroup>
                <BuildToolsEnableTemplateManifests>true</BuildToolsEnableTemplateManifests>
                <BuildToolsEnableAutomaticVersioning>false</BuildToolsEnableAutomaticVersioning>
              </PropertyGroup></Target>
              <Import Project="{Escape(import)}" />
              {targetDefinitions}
            </Project>
            """);
        return project;
    }

    private static ProjectInstance Build(string project, string target, string runtime = "")
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

    private static string Plist(string key, string value) => $"<plist version=\"1.0\"><dict><key>{key}</key><string>{value}</string></dict></plist>";
    private static string Escape(string value) => SecurityElement.Escape(value);
    public void Dispose() => Directory.Delete(directory, true);
}
