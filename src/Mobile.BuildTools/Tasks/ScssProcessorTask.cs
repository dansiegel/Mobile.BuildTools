using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Tasks
{
    public class ScssProcessorTask : BuildToolsTaskBase
    {
        public string[] ScssFiles { get; set; }

        private IEnumerable<ITaskItem> _generatedCssFiles;
        [Output]
        public ITaskItem[] GeneratedCssFiles => _generatedCssFiles?.ToArray() ?? new ITaskItem[0];

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            var filesToProcess = ScssFiles.Where(scss => Path.GetFileName(scss)[0] != '_' && Path.GetExtension(scss) == ".scss" || Path.GetExtension(scss) == ".sass");
            IGenerator<IEnumerable<string>> generator = new ScssGenerator(config)
            {
                InputFiles = filesToProcess
            };
            generator.Execute();
            _generatedCssFiles = generator.Outputs.Select(x => new TaskItem(ProjectCollection.Escape(x)));
        }
    }
}