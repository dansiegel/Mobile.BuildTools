using Microsoft.CodeAnalysis;

namespace Mobile.BuildTools.AppSettings.Diagnostics
{
    internal static class Descriptors
    {
        public static readonly DiagnosticDescriptor MissingMSBuildProperty =
            new DiagnosticDescriptor("MBT100", "Missing MSBuild Property", "The MSBuild property '{0}' was not found", "Configuration", DiagnosticSeverity.Error, true);

        public static readonly DiagnosticDescriptor CreatedClass =
            new DiagnosticDescriptor("MBT200", "Created App Settings class", "Created App Settings class '{0}'", "CodeGen", DiagnosticSeverity.Info, true);

        public static readonly DiagnosticDescriptor MissingAppSettingsProperty =
            new DiagnosticDescriptor("MBT404", "Missing AppSettings Property", "The specified property '{0}' could not be found in either the default buildtools.json environment, the system environment or in a local appsettings.json", "CodeGen", DiagnosticSeverity.Error, true);
    }
}
