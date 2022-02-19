using System.Collections.Generic;
using System.IO;
using CodeGenHelpers;
using Microsoft.CodeAnalysis;
using Mobile.BuildTools.AppSettings.Diagnostics;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Models.Settings;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.AppSettings.Generators
{
    public abstract class GeneratorBase : ISourceGenerator, IBuildConfiguration
    {
        private GeneratorExecutionContext _context;
        private string _configurationPath;
        private string _buildConfiguration;
        private string _projectName;
        private string _projectDirectory;
        private string _rootNamespace;
        private IDictionary<string, string> _props = new Dictionary<string, string>();

        protected string ProjectName => _projectName;
        protected string RootNamespace => _rootNamespace;
        protected string ProjectDirectory => _projectDirectory;
        protected string SolutionDirectory { get; private set; }
        protected BuildToolsConfig Config { get; private set; }
        protected IBuildConfiguration BuildConfiguration => this;

        string IBuildConfiguration.BuildConfiguration => _buildConfiguration;
        bool IBuildConfiguration.BuildingInsideVisualStudio { get; }
        IDictionary<string, string> IBuildConfiguration.GlobalProperties => _props;
        string IBuildConfiguration.ProjectName => ProjectName;
        string IBuildConfiguration.ProjectDirectory => ProjectDirectory;
        string IBuildConfiguration.SolutionDirectory => SolutionDirectory;

        // This shouldn't be an issue for the Source Generator
        string IBuildConfiguration.IntermediateOutputPath => Path.Combine(ProjectDirectory, "obj");
        ILog IBuildConfiguration.Logger => new ConsoleLogger();
        BuildToolsConfig IBuildConfiguration.Configuration { get; }
        Utils.Platform IBuildConfiguration.Platform { get; }

        public void Execute(GeneratorExecutionContext context)
        {
            _context = context;

            if (!(TryGet(context, "MSBuildProjectName", ref _projectName) 
                || TryGet(context, "MSBuildProjectDirectory", ref _projectDirectory)
                || TryGet(context, "RootNamespace", ref _rootNamespace)))
                return;

            SolutionDirectory = EnvironmentAnalyzer.LocateSolution(ProjectDirectory);
            _configurationPath = ConfigHelper.GetConfigurationPath(ProjectDirectory);
            Config = ConfigHelper.GetConfig(_configurationPath);

            Generate();
        }

        protected abstract void Generate();

        private bool TryGet(GeneratorExecutionContext context, string name, ref string value)
        {
            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{name}", out value))
                return true;

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.MissingMSBuildProperty, null, name));
            return false;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // Intentionally Left Empty
        }

        protected bool TryGetTypeSymbol(string fullyQualifiedTypeName, out INamedTypeSymbol typeSymbol)
        {
            typeSymbol = _context.Compilation.GetTypeByMetadataName(fullyQualifiedTypeName);
            return typeSymbol != null;
        }

        protected void AddSource(ClassBuilder builder)
        {
            _context.ReportDiagnostic(Diagnostic.Create(Descriptors.CreatedClass, null, builder.FullyQualifiedName));
            _context.AddSource(builder.FullyQualifiedName, builder.Build());
        }

        protected void ReportDiagnostic(DiagnosticDescriptor descriptor, params string[] messageArgs)
        {
            _context.ReportDiagnostic(Diagnostic.Create(descriptor, null, messageArgs));
        }

        void IBuildConfiguration.SaveConfiguration() =>
            ConfigHelper.SaveConfig(Config, _configurationPath);

        IEnumerable<SettingsConfig> IBuildConfiguration.GetSettingsConfig() =>
            ConfigHelper.GetSettingsConfig(this);
    }
}
