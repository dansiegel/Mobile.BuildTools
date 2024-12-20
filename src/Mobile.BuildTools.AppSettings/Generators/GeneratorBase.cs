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
        private string _buildConfiguration;
        private string _projectName;
        private string _targetFrameworkAssembly;
        private string _rootNamespace;

        protected string ProjectName => _projectName;
        protected string RootNamespace => _rootNamespace;

        protected string BuildConfiguration => _buildConfiguration;

        protected BuildToolsConfig Config { get; private set; }

        protected BuildEnvironment Environment { get; private set; }

        public void Execute(GeneratorExecutionContext context)
        {
            GeneratorContext = context;

            if (!TryGet(context, "MSBuildProjectName", ref _projectName)
                || !TryGet(context, "RootNamespace", ref _rootNamespace)
                || !TryGet(context, "Configuration", ref _buildConfiguration)
                || !TryGet(context, "TargetFrameworkIdentifier", ref _targetFrameworkAssembly))
                return;

            var buildToolsConfig = context.AdditionalFiles.FirstOrDefault(x => Path.GetFileName(x.Path) == Constants.BuildToolsConfigFileName);
            if (buildToolsConfig is null)
                return;

            var json = buildToolsConfig.GetText().ToString();
            Config = JsonSerializer.Deserialize<BuildToolsConfig>(json, ConfigHelper.GetSerializerSettings());

            var buildToolsEnvFile = GeneratorContext.AdditionalFiles.FirstOrDefault(x => Path.GetFileName(x.Path) == Constants.BuildToolsEnvironmentSettings);
            json = buildToolsEnvFile.GetText().ToString();
            Environment = JsonSerializer.Deserialize<BuildEnvironment>(json, new JsonSerializerOptions(JsonSerializerDefaults.General)) ?? new BuildEnvironment();

            try
            {
                Generate();
            }
            catch (Exception ex)
            {
                if(Config.Debug)
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

        private bool TryGet(GeneratorExecutionContext context, string name, ref string value)
        {
            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{name}", out value) && !string.IsNullOrEmpty(value))
            {
                return true;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.MissingMSBuildProperty, null, name));
            return false;
        }

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
