using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace Mobile.BuildTools.Tests.Mocks;

internal sealed class RecordingBuildLogger : ILogger
{
    public List<string> Errors { get; } = new();
    public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Minimal;
    public string Parameters { get; set; }
    public void Initialize(IEventSource eventSource) => eventSource.ErrorRaised += (_, error) => Errors.Add(error.Message);
    public void Shutdown() { }
}
