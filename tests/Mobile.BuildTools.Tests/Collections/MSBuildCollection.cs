using Xunit;

namespace Mobile.BuildTools.Tests.Collections;

// In-process MSBuild owns process-wide state, including the current directory.
// Keep target-graph tests isolated while normal unit tests retain parallelism.
[CollectionDefinition("MSBuild", DisableParallelization = true)]
public sealed class MSBuildCollection
{
}
