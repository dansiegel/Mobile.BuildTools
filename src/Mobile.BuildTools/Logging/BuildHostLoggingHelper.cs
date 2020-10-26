using System;
using Microsoft.Build.Utilities;

namespace Mobile.BuildTools.Logging
{
    public class BuildHostLoggingHelper : ILog
    {
        private TaskLoggingHelper _taskLoggingHelper { get; }

        public BuildHostLoggingHelper(TaskLoggingHelper taskLoggingHelper)
        {
            _taskLoggingHelper = taskLoggingHelper;
        }

        void ILog.LogWarning(string message)
        {
            _taskLoggingHelper.LogWarning(message);
        }

        void ILog.LogError(string message)
        {
            _taskLoggingHelper.LogError(message);
        }

        void ILog.LogWarning(string formattedString, params object[] args)
        {
            _taskLoggingHelper.LogWarning(formattedString, args);
        }

        void ILog.LogMessage(string message)
        {
            _taskLoggingHelper.LogMessage(message);
        }

        void ILog.LogErrorFromException(Exception ex)
        {
            _taskLoggingHelper.LogErrorFromException(ex);
        }

        public static implicit operator BuildHostLoggingHelper(TaskLoggingHelper logger) =>
            new BuildHostLoggingHelper(logger);
    }
}
