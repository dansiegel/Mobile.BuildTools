using Mobile.BuildTools.Build;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Generators
{
    internal abstract class GeneratorBase : IGenerator
    {
        public GeneratorBase(IBuildConfiguration buildConfiguration)
        {
            Build = buildConfiguration;
        }

        public IBuildConfiguration Build { get; }
        public ILog Log => Build.Logger;

        protected abstract void ExecuteInternal();

        public void Execute() => ExecuteInternal();
    }

    internal abstract class GeneratorBase<T> : GeneratorBase, IGenerator<T>
    {
        public GeneratorBase(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        public T Outputs { get; protected set; }
    }
}
