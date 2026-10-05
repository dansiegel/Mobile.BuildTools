using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Tasks;
using Moq;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Tasks;

public sealed class NativeImageManifestFixture : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), nameof(NativeImageManifestFixture), Guid.NewGuid().ToString("N"));
    private readonly Mock<IBuildEngine> engine = new();
    private static readonly XNamespace Android = "http://schemas.android.com/apk/res/android";

    public NativeImageManifestFixture() => Directory.CreateDirectory(directory);

    [Theory]
    [InlineData("MauiIcon")]
    [InlineData("UnoIcon")]
    [InlineData("MauiImage")]
    [InlineData("UnoImage")]
    public void AndroidUsesResizetizerOutputNameIncludingAliasAndRoundSuffix(string kind)
    {
        const string source = "<manifest package=\"com.example\"><application label=\"$AppName$\" /></manifest>";
        var task = Create("android", source, kind);
        task.Images[0].SetMetadata("Link", "Resources/Icons/branded.svg");
        if (kind.EndsWith("Image"))
            task.Images[0].SetMetadata("IsAppIcon", "True");

        Assert.True(task.Execute());

        var application = XDocument.Load(task.GeneratedManifest.ItemSpec).Root.Element("application");
        Assert.Equal("@mipmap/branded", (string)application.Attribute(Android + "icon"));
        Assert.Equal("@mipmap/branded_round", (string)application.Attribute(Android + "roundIcon"));
        Assert.Equal("$AppName$", (string)application.Attribute("label"));
        Assert.Equal(source, File.ReadAllText(task.Manifest.ItemSpec));
        Assert.Equal("preserved", task.GeneratedManifest.GetMetadata("Custom"));
        Assert.Equal(task.GeneratedManifest.ItemSpec, Assert.Single(task.FileWrites).ItemSpec);
        // Both generators write these adaptive XML resource names, regardless of ForegroundFile.
        Assert.Equal("branded.xml", ((string)application.Attribute(Android + "icon")).Split('/').Last() + ".xml");
        Assert.Equal("branded_round.xml", ((string)application.Attribute(Android + "roundIcon")).Split('/').Last() + ".xml");
    }

    [Theory]
    [InlineData("android:icon=\"@drawable/custom\"")]
    [InlineData("android:roundIcon=\"@mipmap/alternate_round\"")]
    public void AndroidPreservesExplicitSelectionWithoutMixingGeneratedDefaults(string attribute)
    {
        var task = Create("android", $"<manifest xmlns:android=\"{Android}\"><application {attribute} /></manifest>");
        Assert.True(task.Execute());
        Assert.Null(task.GeneratedManifest);
        Assert.Empty(task.FileWrites);
        engine.Verify(x => x.LogWarningEvent(It.Is<BuildWarningEventArgs>(e => e.Message.Contains("explicit") && e.Message.Contains("preserved"))), Times.Once());
    }

    [Fact]
    public void AndroidDoesNotDuplicateCorrectReferencesOrChangeSplashTheme()
    {
        var task = Create("android", $"<manifest xmlns:android=\"{Android}\"><application android:icon=\"@mipmap/logo\" android:roundIcon=\"@mipmap/logo_round\" android:theme=\"@style/MySplash\" /></manifest>");
        Assert.True(task.Execute());
        Assert.Null(task.GeneratedManifest);
        engine.Verify(x => x.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.Never());
    }

    [Fact]
    public void AndroidFillsOnlyMissingReferenceAndRetainsNoOpTimestamp()
    {
        var task = Create("android", $"<manifest xmlns:android=\"{Android}\"><application android:icon=\"@mipmap/logo\" /></manifest>");
        Assert.True(task.Execute());
        var output = task.GeneratedManifest.ItemSpec;
        var old = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        File.SetLastWriteTimeUtc(output, old);
        Assert.True(task.Execute());
        Assert.Equal(old, File.GetLastWriteTimeUtc(output));
        File.Delete(output);
        Assert.True(task.Execute());
        Assert.True(File.Exists(output));
    }

    [Theory]
    [InlineData("MauiIcon")]
    [InlineData("UnoIcon")]
    public void AppleSelectsActualCatalogThroughNonOverwritingPartial(string kind)
    {
        var task = Create("apple", "<plist version=\"1.0\"><dict><key>CFBundleName</key><string>Test</string></dict></plist>", kind);
        Assert.True(task.Execute());
        var dictionary = XDocument.Load(task.GeneratedManifest.ItemSpec).Root.Element("dict");
        Assert.Equal("XSAppIconAssets", Assert.Single(dictionary.Elements("key")).Value);
        Assert.Equal("Assets.xcassets/logo.appiconset", Assert.Single(dictionary.Elements("string")).Value);
        Assert.Equal("false", task.GeneratedManifest.GetMetadata("Overwrite"));
        Assert.DoesNotContain("CFBundleIcons", File.ReadAllText(task.GeneratedManifest.ItemSpec));
        Assert.DoesNotContain("UILaunchStoryboardName", File.ReadAllText(task.GeneratedManifest.ItemSpec));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ApplePreservesExplicitCatalogInMainOrPartialManifest(bool partial)
    {
        var task = Create("apple", "<plist><dict /></plist>");
        var selected = Path.Combine(directory, "selected.plist");
        File.WriteAllText(selected, "<plist><dict>\n  <key>XSAppIconAssets</key>\n  <string>Custom.xcassets/other.appiconset</string>\n</dict></plist>");
        if (partial)
            task.PartialManifests = new[] { new TaskItem(selected) };
        else
            task.Manifest = new TaskItem(selected);
        Assert.True(task.Execute());
        Assert.Null(task.GeneratedManifest);
        engine.Verify(x => x.LogWarningEvent(It.Is<BuildWarningEventArgs>(e => e.Message.Contains("XSAppIconAssets"))), Times.Once());
    }

    [Fact]
    public void AppleDoesNotDuplicateExplicitAppManifestEntry()
    {
        var task = Create("apple", "<plist><dict /></plist>");
        var entry = new TaskItem("XSAppIconAssets");
        entry.SetMetadata("Value", "Assets.xcassets/logo.appiconset");
        task.AppManifestEntries = new[] { entry };
        Assert.True(task.Execute());
        Assert.Null(task.GeneratedManifest);
        engine.Verify(x => x.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.Never());
    }

    [Fact]
    public void AppleManualBundleIconSelectionIsNotOverriddenByCatalogDefault()
    {
        var task = Create("apple", "<plist><dict><key>CFBundleIconFiles</key><array><string>custom.png</string></array></dict></plist>");
        Assert.True(task.Execute());
        Assert.Null(task.GeneratedManifest);
        engine.Verify(x => x.LogWarningEvent(It.Is<BuildWarningEventArgs>(e => e.Message.Contains("bundle icon keys"))), Times.Once());
    }

    [Fact]
    public void BinaryAppleManifestIsPreservedWithoutGuessingItsIconSelection()
    {
        var task = Create("apple", "bplist00");
        Assert.True(task.Execute());
        Assert.Null(task.GeneratedManifest);
        Assert.Equal("bplist00", File.ReadAllText(task.Manifest.ItemSpec));
        engine.Verify(x => x.LogWarningEvent(It.Is<BuildWarningEventArgs>(e => e.Message.Contains("binary Apple manifest"))), Times.Once());
    }

    [Theory]
    [InlineData("MauiImage")]
    [InlineData("UnoImage")]
    [InlineData("MauiSplashScreen")]
    [InlineData("UnoSplashScreen")]
    public void OrdinaryImagesAndFrameworkSplashDoNotInventManifestKeys(string kind)
    {
        var task = Create("android", "<manifest><application /></manifest>", kind);
        Assert.True(task.Execute());
        Assert.Null(task.GeneratedManifest);
        task.Platform = "apple";
        Assert.True(task.Execute());
        Assert.Null(task.GeneratedManifest);
    }

    [Fact]
    public void FirstIconWinsLikeNativeResizetizers()
    {
        var task = Create("android", "<manifest><application /></manifest>");
        var second = new TaskItem(Path.Combine(directory, "ignored.svg"));
        second.SetMetadata("MobileBuildToolsItemType", "MauiIcon");
        task.Images = task.Images.Concat(new[] { second }).ToArray();
        Assert.True(task.Execute());
        Assert.Equal("@mipmap/logo", (string)XDocument.Load(task.GeneratedManifest.ItemSpec).Root.Element("application").Attribute(Android + "icon"));
    }

    private GenerateNativeImageManifestTask Create(string platform, string contents, string kind = "MauiIcon")
    {
        var manifest = Path.Combine(directory, platform == "android" ? "AndroidManifest.xml" : "Info.plist");
        File.WriteAllText(manifest, contents);
        var icon = new TaskItem(Path.Combine(directory, "logo.svg"));
        icon.SetMetadata("MobileBuildToolsItemType", kind);
        var input = new TaskItem(manifest);
        input.SetMetadata("Custom", "preserved");
        return new GenerateNativeImageManifestTask { Images = new[] { icon }, Manifest = input, Platform = platform,
            OutputDirectory = "obj/", ProjectDirectory = directory, BuildEngine = engine.Object };
    }

    public void Dispose() => Directory.Delete(directory, true);
}
