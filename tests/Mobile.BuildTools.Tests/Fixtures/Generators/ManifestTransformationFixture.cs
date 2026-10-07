using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Manifests;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Utils;
using Moq;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Generators;

public sealed class ManifestTransformationFixture : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), nameof(ManifestTransformationFixture), Guid.NewGuid().ToString("N"));
    private readonly Mock<ILog> logger = new();
    private readonly BuildToolsConfig configuration = new()
    {
        Manifests = new() { Token = "$", VariablePrefix = "Manifest_" },
        Environment = new() { Defaults = new Dictionary<string, string>() },
        AutomaticVersioning = new() { Behavior = VersionBehavior.Off }
    };

    public ManifestTransformationFixture() => Directory.CreateDirectory(directory);

    [Theory]
    [InlineData("literal $& $1 $$ and <xml> & \"quotes\" 'apostrophes'")]
    [InlineData("line one\nline two\\path")]
    [InlineData("")]
    [InlineData("$Unresolved$ stays literal")]
    public void XmlReplacementsAreLiteralAndEscaped(string value)
    {
        configuration.Environment.Defaults["Manifest_Value.+"] = value;
        var source = "<manifest package=\"com.example\"><application label=\"$Value.+$\"><name>$Value.+$</name></application></manifest>";
        var generator = Create("AndroidManifest.xml", source, Platform.Android);
        generator.Execute();
        var document = XDocument.Load(generator.Outputs);
        Assert.Equal(value, document.Root.Element("application").Attribute("label").Value);
        Assert.Equal(value, document.Root.Element("application").Element("name").Value);
        Assert.Equal(source, File.ReadAllText(generator.ManifestInputPath));
    }

    [Fact]
    public void ReplacementDoesNotTreatTokenNamesAsRegex()
    {
        configuration.Environment.Defaults["Manifest_A.+"] = "first";
        configuration.Environment.Defaults["Manifest_ABC"] = "second";
        var generator = Create("Package.appxmanifest", "<Package Name=\"$A.+$\" Other=\"$ABC$\" />", Platform.Windows);
        generator.Execute();
        var document = XDocument.Load(generator.Outputs);
        Assert.Equal("first", document.Root.Attribute("Name").Value);
        Assert.Equal("second", document.Root.Attribute("Other").Value);
    }

    [Fact]
    public void WindowsFrameworkPlaceholdersRemainForTheFrameworkTransformer()
    {
        configuration.Manifests.MissingTokensAsErrors = true;
        var generator = Create("Package.appxmanifest", "<Package Name=\"$placeholder$\" Executable=\"$targetnametoken$.exe\" EntryPoint=\"$targetentrypoint$\" />", Platform.Windows);
        generator.Execute();
        Assert.Contains("$placeholder$", File.ReadAllText(generator.Outputs));
        Assert.Contains("$targetnametoken$", File.ReadAllText(generator.Outputs));
        Assert.Contains("$targetentrypoint$", File.ReadAllText(generator.Outputs));
        logger.Verify(log => log.LogError(It.IsAny<string>()), Times.Never());
        logger.Verify(log => log.LogWarning(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public void PlistTypedTokensAreReplacedBeforeValidation()
    {
        configuration.Environment.Defaults["Manifest_Count"] = "42";
        configuration.Environment.Defaults["Manifest_Ratio"] = "3.25";
        var generator = Create("Info.plist", "<plist version=\"1.0\"><dict><key>Count</key><integer>$Count$</integer><key>Ratio</key><real>$Ratio$</real><key>Enabled</key><true/></dict></plist>", Platform.iOS);
        generator.Execute();
        var document = XDocument.Load(generator.Outputs);
        Assert.Equal("42", document.Descendants("integer").Single().Value);
        Assert.Equal("3.25", document.Descendants("real").Single().Value);
        Assert.Single(document.Descendants("true"));
    }

    [Theory]
    [InlineData("integer", "not-an-int")]
    [InlineData("real", "NaN")]
    [InlineData("data", "bad-base64")]
    [InlineData("date", "bad-date")]
    public void InvalidTypedValuesFailWithoutExposingValues(string element, string value)
    {
        configuration.Environment.Defaults["Manifest_Value"] = value;
        var generator = Create("Info.plist", $"<plist><dict><key>Value</key><{element}>$Value$</{element}></dict></plist>", Platform.iOS);
        var exception = Assert.Throws<InvalidDataException>(generator.Execute);
        Assert.DoesNotContain(value, exception.Message);
        Assert.False(File.Exists(generator.ManifestOutputPath));
    }

    [Fact]
    public void JsonManifestPreservesTypesAndEscapesStrings()
    {
        const string value = "quoted \"value\" \\ path \n $& <&>";
        configuration.Environment.Defaults["Manifest_Value"] = value;
        var generator = Create("manifest.webmanifest", "{\"name\":\"$Value$\",\"count\":2,\"enabled\":true,\"icons\":[{\"src\":\"$Value$\"}]}", Platform.WebAssembly);
        generator.Execute();
        using var document = JsonDocument.Parse(File.ReadAllText(generator.Outputs));
        Assert.Equal(value, document.RootElement.GetProperty("name").GetString());
        Assert.Equal(2, document.RootElement.GetProperty("count").GetInt32());
        Assert.True(document.RootElement.GetProperty("enabled").GetBoolean());
        Assert.Equal(value, document.RootElement.GetProperty("icons")[0].GetProperty("src").GetString());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MissingValuesRespectConfiguredSeverity(bool error)
    {
        configuration.Manifests.MissingTokensAsErrors = error;
        var generator = Create("Package.appxmanifest", "<Package Name=\"$Missing$\" />", Platform.Windows);
        generator.Execute();
        Assert.Contains("$Missing$", File.ReadAllText(generator.Outputs));
        logger.Verify(log => log.LogError(It.IsAny<string>()), error ? Times.Once() : Times.Never());
        logger.Verify(log => log.LogWarning(It.IsAny<string>()), error ? Times.Never() : Times.Once());
    }

    [Fact]
    public void DebugLoggingDoesNotContainValues()
    {
        const string privateValue = "private-value-unique-to-this-test";
        configuration.Debug = true;
        configuration.Environment.Defaults["Manifest_Value"] = privateValue;
        var generator = Create("Info.plist", "<plist><dict><key>Name</key><string>$Value$</string></dict></plist>", Platform.iOS);
        generator.Execute();
        Assert.DoesNotContain(logger.Invocations.SelectMany(call => call.Arguments).OfType<string>(), value => value.Contains(privateValue));
    }

    [Fact]
    public void IdenticalBuildPreservesOutputTimestampAndChangedValuesInvalidate()
    {
        configuration.Environment.Defaults["Manifest_Value"] = "first";
        var generator = Create("Package.appxmanifest", "<Package Name=\"$Value$\" />", Platform.Windows);
        generator.Execute();
        var timestamp = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        File.SetLastWriteTimeUtc(generator.Outputs, timestamp);
        generator.Execute();
        Assert.Equal(timestamp, File.GetLastWriteTimeUtc(generator.Outputs));
        configuration.Environment.Defaults["Manifest_Value"] = "second";
        generator.Execute();
        Assert.Equal("second", XDocument.Load(generator.Outputs).Root.Attribute("Name").Value);
        Assert.NotEqual(timestamp, File.GetLastWriteTimeUtc(generator.Outputs));
    }

    [Fact]
    public void DefaultDelimiterIsLiteralDoubleDollar()
    {
        configuration.Manifests.Token = null;
        configuration.Environment.Defaults["Manifest_Name"] = "replaced";
        var generator = Create("Package.appxmanifest", "<Package Name=\"$$Name$$\" />", Platform.Windows);
        generator.Execute();
        Assert.Equal("replaced", XDocument.Load(generator.Outputs).Root.Attribute("Name").Value);
    }

    [Fact]
    public void PackageIdComesFromGeneratedAndroidManifest()
    {
        configuration.Environment.Defaults["Manifest_Package"] = "com.example.generated";
        var generator = Create("AndroidManifest.xml", "<manifest package=\"$Package$\" />", Platform.Android);
        generator.Execute();
        Assert.Equal("com.example.generated", generator.GetBundId());
    }

    [Fact]
    public void ExternalEntitiesAreNeverResolved()
    {
        var secret = Path.Combine(directory, "external-secret.txt");
        File.WriteAllText(secret, "must-not-be-read");
        var generator = Create("Info.plist", $"<!DOCTYPE plist [<!ENTITY external SYSTEM '{new Uri(secret).AbsoluteUri}'>]><plist><dict><key>Value</key><string>&external;</string></dict></plist>", Platform.iOS);
        Assert.Throws<System.Xml.XmlException>(generator.Execute);
        Assert.False(File.Exists(generator.ManifestOutputPath));
    }

    private BaseTemplatedManifestGenerator Create(string name, string source, Platform platform)
    {
        var build = new Mock<IBuildConfiguration>();
        build.SetupGet(value => value.Configuration).Returns(configuration);
        build.SetupGet(value => value.ProjectDirectory).Returns(directory);
        build.SetupGet(value => value.SolutionDirectory).Returns(directory);
        build.SetupGet(value => value.BuildConfiguration).Returns("Debug");
        build.SetupGet(value => value.Platform).Returns(platform);
        build.SetupGet(value => value.Logger).Returns(logger.Object);
        var input = Path.Combine(directory, name);
        File.WriteAllText(input, source);
        var generator = ManifestGeneratorFactory.Create(input, build.Object);
        generator.ManifestInputPath = input;
        generator.ManifestOutputPath = Path.Combine(directory, "obj", name);
        return generator;
    }

    public void Dispose() => Directory.Delete(directory, true);
}
