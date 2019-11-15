using Mobile.BuildTools.Build;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Generators
{
    internal interface IGenerator
    {
        IBuildConfiguration Build { get; }

        ILog Log { get; }

        void Execute();
    }

    internal interface IGenerator<T> : IGenerator
    {
        T Outputs { get; }
    }
}
