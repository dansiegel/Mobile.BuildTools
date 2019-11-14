using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Mobile.BuildTools.Tasks
{
    public class ImageResizerTask : Task
    {
        public ITaskItem[] Images { get; set; }

        public override bool Execute()
        {
            throw new NotImplementedException();
        }
    }
}
