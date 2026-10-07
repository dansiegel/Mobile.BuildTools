using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using E2E.Tests.Helpers;
using Xunit;

namespace E2E.Tests.Fixtures;

public sealed class AppManifestsFixture
{
    private const string ExpectedLiteral = "Build smoke <&> \"quotes\" 'apostrophes' \\path $& $1 $$NotAToken$$\nsecond line";

    [Fact]
    public void BuiltConsumerContainsXmlWithLiteralTextAndAttributes()
    {
        var manifest = XDocument.Parse(ConsumerResources.ReadText("Outputs.Manifest.xml"));
        var application = manifest.Root.Element("application");
        Assert.Equal("com.example.packagesmoke", manifest.Root.Attribute("package").Value);
        Assert.Equal(ExpectedLiteral, application.Attribute(XName.Get("label", "urn:e2e:application")).Value);
        Assert.Equal(ExpectedLiteral, application.Element("name").Value);
        Assert.Equal(string.Empty, application.Attribute("empty").Value);
        Assert.Equal("literal-key", application.Element("literalKey").Value);
    }

    [Fact]
    public void BuiltConsumerContainsPlistWithLiteralStringsAndTypedValues()
    {
        var manifest = XDocument.Parse(ConsumerResources.ReadText("Outputs.Info.plist"));
        var values = manifest.Root.Element("dict").Elements("key")
            .ToDictionary(key => key.Value, key => key.ElementsAfterSelf().First());

        Assert.Equal("com.example.packagesmoke", values["CFBundleIdentifier"].Value);
        Assert.Equal("string", values["CFBundleDisplayName"].Name.LocalName);
        Assert.Equal(ExpectedLiteral, values["CFBundleDisplayName"].Value);
        Assert.Equal(string.Empty, values["Empty"].Value);
        Assert.Equal("integer", values["Count"].Name.LocalName);
        Assert.Equal("42", values["Count"].Value);
        Assert.Equal("real", values["Ratio"].Name.LocalName);
        Assert.Equal("3.25", values["Ratio"].Value);
        Assert.Equal("true", values["Enabled"].Name.LocalName);
    }

    [Fact]
    public void BuiltConsumerContainsJsonWithLiteralStringsAndUnchangedTypes()
    {
        using var manifest = JsonDocument.Parse(ConsumerResources.ReadText("Outputs.manifest.json"));
        var root = manifest.RootElement;
        Assert.Equal(ExpectedLiteral, root.GetProperty("name").GetString());
        Assert.Equal(ExpectedLiteral, root.GetProperty("icons")[0].GetProperty("src").GetString());
        Assert.Equal(string.Empty, root.GetProperty("literal-key").GetString());
        Assert.Equal(42, root.GetProperty("count").GetInt32());
        Assert.Equal(3.25, root.GetProperty("ratio").GetDouble());
        Assert.True(root.GetProperty("enabled").GetBoolean());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("optional").ValueKind);
    }

    [Theory]
    [InlineData("Manifest.xml")]
    [InlineData("Info.plist")]
    [InlineData("manifest.json")]
    public void ConsumerSourceManifestsKeepTheirTokens(string name)
    {
        var source = ConsumerResources.ReadText($"Inputs.{name}");
        Assert.Contains("$$SmokeLiteral$$", source);
        Assert.Contains("$$SmokeEmpty$$", source);
        Assert.DoesNotContain("Build smoke", source);
    }
}
