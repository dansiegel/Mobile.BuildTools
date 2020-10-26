using System;
using Mobile.BuildTools.Logging;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Mocks
{
    public class XunitLog : ILog
    {
        private ITestOutputHelper _testOutputHelper { get; }

        public XunitLog(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        void ILog.LogMessage(string message)
        {
            _testOutputHelper.WriteLine(message);
        }

        void ILog.LogWarning(string message)
        {
            _testOutputHelper.WriteLine(message);
        }

        void ILog.LogWarning(string formattedString, params object[] args)
        {
            _testOutputHelper.WriteLine(formattedString, args);
        }

        void ILog.LogErrorFromException(Exception ex)
        {
            _testOutputHelper.WriteLine(ex.ToString());
        }

        void ILog.LogError(string message)
        {
            _testOutputHelper.WriteLine(message);
        }
    }
}
