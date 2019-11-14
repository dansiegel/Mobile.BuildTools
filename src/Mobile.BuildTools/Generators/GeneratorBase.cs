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

        protected abstract void Execute();

        void IGenerator.Execute() => Execute();
    }
}
