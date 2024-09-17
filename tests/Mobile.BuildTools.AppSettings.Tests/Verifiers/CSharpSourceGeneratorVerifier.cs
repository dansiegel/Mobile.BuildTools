// Uncomment the following line to write expected files to disk
#define WRITE_EXPECTED

#if WRITE_EXPECTED
#warning WRITE_EXPECTED is fine for local builds, but should not be merged to the main branch.
#endif

using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Mobile.BuildTools.AppSettings.Tests.Verifiers;

public static partial class CSharpSourceGeneratorVerifier<TSourceGenerator>
        where TSourceGenerator : ISourceGenerator, new()
{
    public class Test : CSharpSourceGeneratorTest<EmptySourceGeneratorProvider, DefaultVerifier>
    {
        private readonly string? _testFile;
        private readonly string? _testMethod;

        public Test([CallerFilePath] string? testFile = null, [CallerMemberName] string? testMethod = null)
        {
            CompilerDiagnostics = CompilerDiagnostics.Errors;

            _testFile = testFile;
            _testMethod = testMethod;

#if WRITE_EXPECTED
            TestBehaviors |= TestBehaviors.SkipGeneratedSourcesCheck;
#endif
        }

        public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Default;

        protected override IEnumerable<Type> GetSourceGenerators()
        {
            yield return typeof(TSourceGenerator);
        }

        protected override CompilationOptions CreateCompilationOptions()
        {
            var compilationOptions = (CSharpCompilationOptions)base.CreateCompilationOptions();
            return compilationOptions
                .WithAllowUnsafe(false)
                .WithWarningLevel(99)
                .WithSpecificDiagnosticOptions(compilationOptions.SpecificDiagnosticOptions.SetItem("CS8019", ReportDiagnostic.Warn));
        }

        protected override ParseOptions CreateParseOptions() =>
            ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(LanguageVersion);

        protected override async Task<(Compilation compilation, ImmutableArray<Diagnostic> generatorDiagnostics)> GetProjectCompilationAsync(Project project, IVerifier verifier, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(project);
            Assert.NotNull(_testMethod);
            Assert.NotNull(_testFile);

            var resourceDirectory = Path.Combine(new FileInfo(_testFile).DirectoryName!, "Expected", _testMethod);

            var (compilation, generatorDiagnostics) = await base.GetProjectCompilationAsync(project, verifier, cancellationToken);
            var expectedNames = new HashSet<string>();
            foreach (var tree in compilation.SyntaxTrees.Skip(project.DocumentIds.Count))
            {
                WriteTreeToDiskIfNecessary(tree, resourceDirectory);
                expectedNames.Add(Path.GetFileName(tree.FilePath));
            }

            foreach (var name in GetType().Assembly.GetManifestResourceNames())
            {
                if (!expectedNames.Contains(name))
                {
                    throw new InvalidOperationException($"Unexpected test resource: {name}");
                }
            }

            return (compilation, generatorDiagnostics);
        }

        public Test AddGeneratedSources([CallerMemberName] string? testMethod = null)
        {
            Assert.NotNull(testMethod);

            var expectedFiles = new DirectoryInfo(Path.Combine("Expected", testMethod)).EnumerateFiles("*.g.cs");
            foreach (var file in expectedFiles)
            {
                using var resourceStream = file.OpenRead() ?? throw new InvalidOperationException();
                using var reader = new StreamReader(resourceStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: true);
                var name = file.Name;
                TestState.GeneratedSources.Add((typeof(TSourceGenerator), name, reader.ReadToEnd()));
            }

            return this;
        }

        [Conditional("WRITE_EXPECTED")]
        private static void WriteTreeToDiskIfNecessary(SyntaxTree tree, string resourceDirectory)
        {
            if (tree.Encoding is null)
            {
                throw new ArgumentException("Syntax tree encoding was not specified");
            }

            var name = Path.GetFileName(tree.FilePath);
            var filePath = Path.Combine(resourceDirectory, name);
            Directory.CreateDirectory(resourceDirectory);
            File.WriteAllText(filePath, tree.GetText().ToString(), tree.Encoding);
        }
    }
}
