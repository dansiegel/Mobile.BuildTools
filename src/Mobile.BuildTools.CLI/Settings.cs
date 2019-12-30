using System;
using System.Collections.Generic;

namespace Mobile.BuildTools
{
    internal static class Settings
    {
        public const string CommandName = "buildtools";

        //--- Class Fields ---
        public static VerboseLevel VerboseLevel = VerboseLevel.Exceptions;
        public static bool UseAnsiConsole = true;
        private static readonly IList<(string Message, Exception Exception)> _errors = new List<(string Message, Exception Exception)>();

        //--- Class Properties ---
        public static int ErrorCount => _errors.Count;
        public static bool HasErrors => _errors.Count > 0;

        //--- Class Methods ---
        public static void ShowErrors()
        {
            foreach (var error in _errors)
            {
                if ((error.Exception != null) && (VerboseLevel >= VerboseLevel.Exceptions))
                {
                    Console.WriteLine("ERROR: " + error.Message + Environment.NewLine + error.Exception);
                }
                else
                {
                    Console.WriteLine("ERROR: " + error.Message);
                }
            }
        }

        public static void LogWarn(string message)
            => Console.WriteLine("WARNING: " + message);

        public static void LogError(string message, Exception exception = null)
            => _errors.Add((Message: message, Exception: exception));

        public static void LogError(Exception exception)
            => LogError($"internal error: {exception.Message}", exception);
    }
}
