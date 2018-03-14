using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Generators
{
    internal interface IGenerator
    {
        ILog Log { get; set; }

        bool? DebugOutput { get; set; }

        void Execute();
    }
}
