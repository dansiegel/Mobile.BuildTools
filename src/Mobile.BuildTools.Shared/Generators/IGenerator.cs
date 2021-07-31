using Mobile.BuildTools.Build;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Generators
{
    public interface IGenerator
    {
        IBuildConfiguration Build { get; }

        ILog Log { get; }

        void Execute();
    }

    public interface IGenerator<T> : IGenerator
    {
        T Outputs { get; }
    }
}
