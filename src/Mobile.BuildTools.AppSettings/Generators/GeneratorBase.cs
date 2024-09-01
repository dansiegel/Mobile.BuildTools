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
        private string _configurationPath;
        private string _buildConfiguration;
        private string _intermediateOutputDir;
        private string _projectName;
        private string _projectDirectory;
        private string _targetFrameworkAssembly;
        private string _rootNamespace;
        private IDictionary<string, string> _props = new Dictionary<string, string>();

        protected string ProjectName => _projectName;
        protected string RootNamespace => _rootNamespace;
        protected string ProjectDirectory => _projectDirectory;
        protected string SolutionDirectory { get; private set; }
        protected BuildToolsConfig Config { get; private set; }

        public void Execute(GeneratorExecutionContext context)
        {
            GeneratorContext = context;

            if (!TryGet(context, "MSBuildProjectName", ref _projectName)
                || !TryGet(context, "MSBuildProjectDirectory", ref _projectDirectory)
                || !TryGet(context, "RootNamespace", ref _rootNamespace)
                || !TryGet(context, "Configuration", ref _buildConfiguration)
                || !TryGet(context, "TargetFrameworkIdentifier", ref _targetFrameworkAssembly)
                || !TryGet(context, "IntermediateOutputPath", ref _intermediateOutputDir))
                return;

            //SolutionDirectory = EnvironmentAnalyzer.LocateSolution(ProjectDirectory);
            //_configurationPath = ConfigHelper.GetConfigurationPath(ProjectDirectory);
            var buildToolsConfig = context.AdditionalFiles.FirstOrDefault(x => Path.GetFileName(x.Path) == Constants.BuildToolsConfigFileName);
            if (buildToolsConfig is null)
                return;

            var json = buildToolsConfig.GetText().ToString();
            Config = JsonSerializer.Deserialize<BuildToolsConfig>(json, ConfigHelper.GetSerializerSettings());

            try
            {
                Generate();
            }
            catch (System.Exception ex)
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
            GeneratorContext.AddSource(builder.FullyQualifiedName, builder.Build());
        }

        protected void ReportDiagnostic(DiagnosticDescriptor descriptor, params string[] messageArgs)
        {
            GeneratorContext.ReportDiagnostic(Diagnostic.Create(descriptor, null, messageArgs));
        }
    }
}
