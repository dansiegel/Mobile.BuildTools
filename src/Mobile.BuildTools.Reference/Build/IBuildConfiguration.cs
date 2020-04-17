using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Build
{
    internal interface IBuildConfiguration
    {
        string ProjectDirectory { get; }
        string IntermediateOutputPath { get; }
        Platform Platform { get; }
        ILog Logger { get; }
    }
}
