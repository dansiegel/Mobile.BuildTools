using System;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Tasks
{
    public class GenerateGitReleaseNotesTask : Microsoft.Build.Utilities.Task
    {
        public string OutputDirectory { get; set; }
        public int DateLookback { get; set; }
        public int MaxCommit { get; set; }
        public int CharacterLimit { get; set; }

        public override bool Execute()
        {
            var fromDate = DateTime.Now.AddDays(DateLookback > 0 ? DateLookback * -1 : DateLookback);
            var generator = new ReleaseNotesGenerator()
            {
                CharacterLimit = CharacterLimit,
                FromDate = fromDate,
                Log = (BuildHostLoggingHelper)Log,
                MaxCommit = MaxCommit,
                OutputDirectory = OutputDirectory
            };
            generator.Execute();

            return true;
        }
    }
}
