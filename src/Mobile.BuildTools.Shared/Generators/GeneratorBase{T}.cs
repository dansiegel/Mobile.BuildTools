using Mobile.BuildTools.Build;

namespace Mobile.BuildTools.Generators
{
    public abstract class GeneratorBase<T> : GeneratorBase, IGenerator<T>
    {
        public GeneratorBase(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        public T Outputs { get; protected set; }
    }
}
