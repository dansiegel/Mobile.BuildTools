using System;
namespace Mobile.BuildTools.Logging
{
    public interface ILog
    {
        void LogMessage(string message);
        void LogError(string message);

        void LogWarning(string message);

        void LogWarning(string formattedString, params object[] args);

        void LogErrorFromException(Exception ex);
    }
}
