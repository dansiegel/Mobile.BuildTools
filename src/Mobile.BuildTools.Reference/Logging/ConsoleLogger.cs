using System;

namespace Mobile.BuildTools.Logging
{
    internal class ConsoleLogger : ILog
    {
        public void LogError(string message) =>
            Console.WriteLine(message);

        public void LogErrorFromException(Exception ex) =>
            Console.WriteLine(ex);

        public void LogMessage(string message) =>
            Console.WriteLine(message);

        public void LogWarning(string message) =>
            Console.WriteLine(message);

        public void LogWarning(string formattedString, params object[] args) =>
            Console.WriteLine(formattedString, args);
    }
}
