using System;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

namespace Mobile.BuildTools.Commands
{
    public abstract class CliBase //: IRegisterable
    {

        //--- Class Fields ---
        private static VersionInfo _version;

        //--- Class Constructor ---
        static CliBase()
        {
            _version = VersionInfo.Parse(typeof(CliBase).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
        }

        //public abstract void Register(CommandLineApplication app);

        //--- Class Properties ---
        protected static VersionInfo Version => _version;
        protected static bool HasErrors => Settings.HasErrors;

        //--- Class Methods ---
        protected static void LogWarn(string message)
            => Settings.LogWarn(message);

        protected static void LogError(string message, Exception exception = null)
            => Settings.LogError(message, exception);

        protected static void LogError(Exception exception)
            => Settings.LogError(exception);
    }
}
