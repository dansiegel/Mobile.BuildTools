using System;
using Microsoft.Build.Framework;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Secrets;

namespace Mobile.BuildTools.Tasks
{
    public class SecretsJsonTask : BuildToolsTaskBase
    {
        private ITaskItem[] _generatedCodeFiles;

        public string RootNamespace { get; set; }

        [Output]
        public ITaskItem[] GeneratedCodeFiles => _generatedCodeFiles ?? Array.Empty<ITaskItem>();

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            var generator = new SecretsClassGenerator(config)
            {
                BaseNamespace = RootNamespace,
            };
            generator.Execute();
            _generatedCodeFiles = new[] { generator.Outputs };
        }
    }
}