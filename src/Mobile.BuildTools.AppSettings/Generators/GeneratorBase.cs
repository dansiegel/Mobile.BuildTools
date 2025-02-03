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
                ReportError("MBT500", "DEBUG - Unhandled Error",
                                "An Unhandled Generator Error Occurred: {0} - {1}",
                                ex, "DEBUG");
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

        protected void Report(string id, string title, string messageFormat, string category, DiagnosticSeverity severity, bool enabledByDefault = true, params object[] messageParameters)
        {
            var descriptor = new DiagnosticDescriptor(id, title, messageFormat, category, severity, enabledByDefault);
            ReportDiagnostic(descriptor, messageParameters);
        }

        protected void DebugDiagnostic(string title, string messageFormat, params object[] messageParameters)
        {
            if (!Environment.Debug)
                return;

            Report("MBT0042", title, messageFormat, "Debug", DiagnosticSeverity.Info, true, messageParameters);
        }

        protected void ReportError(string id, string title, string messageFormat, Exception exception, string category = "Debug") =>
            Report(id, title, messageFormat, category, DiagnosticSeverity.Error, Environment.Debug, exception.Message, exception.StackTrace);

        protected void AddSource(ClassBuilder builder)
        {
            GeneratorContext.ReportDiagnostic(Diagnostic.Create(Descriptors.CreatedClass, null, builder.FullyQualifiedName));
            var source = builder.Build();
            GeneratorContext.AddSource(builder.FullyQualifiedName, source);
        }

        protected void ReportDiagnostic(DiagnosticDescriptor descriptor, params object[] messageArgs)
        {
            GeneratorContext.ReportDiagnostic(Diagnostic.Create(descriptor, null, messageArgs));
        }
    }
}
