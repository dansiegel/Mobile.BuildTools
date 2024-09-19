using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using Mobile.BuildTools.AppSettings.Diagnostics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;
using Verify = Mobile.BuildTools.AppSettings.Tests.Verifiers.CSharpSourceGeneratorVerifier<Mobile.BuildTools.AppSettings.Generators.AppSettingsGenerator>;

namespace Mobile.BuildTools.AppSettings.Tests;

public class AppSettingsGeneratorFixture
{
    [Fact]
    public async Task AddsSimpleProperty()
    {
        
        await Test();
    }

    private static Task Test([CallerMemberName] string? method = null)
    {
        Assert.NotNull(method);
        var expected = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Expected", method));
        var expectedFiles = expected.GetFiles("*.cs").Select(x => x.Name).ToArray();
        var sources = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Sources", method));
        var sourceFiles = sources.GetFiles("*").Select(x => x.Name).ToArray();
        return Test(expectedFiles, sourceFiles, method);
    }

    private static Task Test(string[] generatedSources, string[] additionalFiles, [CallerMemberName] string? method = null)
    {
        Assert.NotNull(method);

        var test = new Verify.Test(testMethod: method)
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        AddMSBuildProperties(test, ("MSBuildProjectName", test.TestState.Name),
            ("RootNamespace", test.TestState.AssemblyName),
            ("Configuration", "Debug"),
            ("TargetFrameworkIdentifier", "net8.0"));

        test.TestState.GeneratedSources.AddRange(GetGeneratedFiles(generatedSources, method));
        test.TestState.AdditionalFiles.AddRange(GetAdditionalFiles(additionalFiles, method));

        var diagnostics = test.TestState.GeneratedSources.Select(x => new DiagnosticResult(Diagnostics.Descriptors.CreatedClass)
            .WithMessage(string.Format(Diagnostics.Descriptors.CreatedClass.MessageFormat.ToString(), Path.GetFileNameWithoutExtension(x.filename))));
        test.TestState.ExpectedDiagnostics.AddRange(diagnostics);

        return test.RunAsync();
    }

    private static void AddMSBuildProperties(Verify.Test test, params(string, string)[] buildProperties)
    {
        var sb = new StringBuilder();
        sb.AppendLine("is_global = true");
        foreach(var (property, value) in buildProperties)
        {
            sb.AppendLine($"build_property.{property} = {value}");
        }

        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", sb.ToString()));
    }

    private static SourceFileCollection GetAdditionalFiles(string[] sourceFiles, string method)
    {
        var files = new SourceFileCollection();
        foreach(var file in sourceFiles)
        {
            files.Add(GetAdditionalFile(file, method));
        }

        if (!sourceFiles.Contains(Constants.BuildToolsEnvironmentSettings))
        {
            files.Add(GetAdditionalFile(Constants.BuildToolsEnvironmentSettings, method));
        }

        if (!sourceFiles.Contains(Constants.BuildToolsConfigFileName))
        {
            files.Add(GetAdditionalFile(Constants.BuildToolsConfigFileName, method));
        }

        return files;
    }

    private static (string, SourceText) GetAdditionalFile(string sourceFile, string method)
    {
        var path = Path.Combine(Environment.CurrentDirectory, "Sources", method, sourceFile);
        Assert.True(File.Exists(path));

        return (path, GetSourceText(path));
    }

    private static SourceFileCollection GetGeneratedFiles(string[] generatedSources, string method)
    {
        var files = new SourceFileCollection();
        foreach(var file in generatedSources)
        {
            files.Add(GetGeneratedFile(file, method));
        }

        return files;
    }

    private static (string, SourceText) GetGeneratedFile(string filename, string method)
    {
        var path = Path.Combine(Environment.CurrentDirectory, "Expected", method, filename);
        Assert.True(File.Exists(path), $"Path does not exist: '{path}'");

        return (path, GetSourceText(path));
    }

    private static SourceText GetSourceText(string path)
    {
        using var reader = new StreamReader(path);
        var source = /*Regex.Replace(*/reader.ReadToEnd()/*, @"\r\n|\n\r|\n|\r", Environment.NewLine)*/;
        return SourceText.From(source, Encoding.UTF8);
    }
}