using System.Text.Json;
using CodeGenHelpers;
using Microsoft.CodeAnalysis;
using Mobile.BuildTools.AppSettings.Diagnostics;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.AppSettings.Generators
{
    public abstract class GeneratorBase : ISourceGenerator
    {
        protected GeneratorExecutionContext GeneratorContext { get; private set; }

        protected BuildEnvironment Environment { get; private set; }

        public void Execute(GeneratorExecutionContext context)
        {
            GeneratorContext = context;

            var buildToolsEnvFile = GeneratorContext.AdditionalFiles.FirstOrDefault(x => Path.GetFileName(x.Path) == Constants.BuildToolsEnvironmentSettings);
            var json = buildToolsEnvFile.GetText().ToString();
            Environment = JsonSerializer.Deserialize<BuildEnvironment>(json, new JsonSerializerOptions(JsonSerializerDefaults.General)) ?? new BuildEnvironment();

            try
            {
                Generate();
            }
            catch (Exception ex)
            {
                if (Environment.Debug)
                    context.ReportDiagnostic
                        (Diagnostic.Create(
                            new DiagnosticDescriptor(
                                "MBT500",
                                "DEBUG - Unhandled Error",
                                "An Unhandled Generator Error Occurred: {0} - {1}",
                                "DEBUG",
                                DiagnosticSeverity.Error,
                                true),
                            null,
                            ex.Message, ex.StackTrace));
            }
        }

        protected abstract void Generate();

        public void Initialize(GeneratorInitializationContext context)
        {
            // Intentionally Left Empty
        }

        protected bool TryGetTypeSymbol(string fullyQualifiedTypeName, out INamedTypeSymbol typeSymbol)
        {
            typeSymbol = GeneratorContext.Compilation.GetTypeByMetadataName(fullyQualifiedTypeName);
            return typeSymbol != null;
        }

        protected void AddSource(ClassBuilder builder)
        {
            GeneratorContext.ReportDiagnostic(Diagnostic.Create(Descriptors.CreatedClass, null, builder.FullyQualifiedName));
            var source = builder.Build();
            GeneratorContext.AddSource(builder.FullyQualifiedName, source);
        }

        protected void ReportDiagnostic(DiagnosticDescriptor descriptor, params string[] messageArgs)
        {
            GeneratorContext.ReportDiagnostic(Diagnostic.Create(descriptor, null, messageArgs));
        }
    }
}
