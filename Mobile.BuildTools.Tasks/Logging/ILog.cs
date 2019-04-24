using System;
namespace Mobile.BuildTools.Logging
{
    public interface ILog
    {
        void LogMessage(string message);

        void LogWarning(string message);

        void LogWarning(string formattedString, params object[] args);
    }
}
