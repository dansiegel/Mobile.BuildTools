using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Mobile.BuildTools.Build;

namespace Mobile.BuildTools.Generators
{
    internal class ReleaseNotesGenerator : GeneratorBase
    {
        public ReleaseNotesGenerator(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        protected override void Execute()
        {
            var releaseNotesOptions = Build.Configuration.ReleaseNotes;

            if (releaseNotesOptions.Disable) return;

            var branchName = GetBranchName();
            if (string.IsNullOrWhiteSpace(branchName))
            {
                Log.LogMessage("It appears the git repo is in a disconnected state, unable to determine the current branch name");
                return;
            }

            var fromDate = DateTime.Now.AddDays(releaseNotesOptions.MaxDays > 0 ? releaseNotesOptions.MaxDays * -1 : releaseNotesOptions.MaxDays);

            //"git log --no-merges --since="$TODAY 00:00:00" --format=" % cd" --date=short";
            var dates = GetCommitDates(fromDate);
            var notes = string.Empty;
            var characterLimitExceeded = false;
            var commits = 0;
            
            foreach (var date in dates)
            {
                // 2018-07-25
                //"git log --no-merges --format=" * % s" --since="2018-07-25 00:00:00" --until="2018-07-25 24:00:00""
                if (commits > Build.Configuration.ReleaseNotes.MaxCommit || characterLimitExceeded)
                {
                    Log.LogMessage("Maximimum limits reaches. Truncating Release Notes.");
                    break;
                }
                    
                foreach (var line in GetCommitMessages(date))
                {
                    if (releaseNotesOptions.MaxCommit > 0 && commits++ > releaseNotesOptions.MaxCommit)
                        break;
                    if(releaseNotesOptions.CharacterLimit > 0 && line.Length + notes.Length > releaseNotesOptions.CharacterLimit)
                    {
                        characterLimitExceeded = true;
                        break;
                    }
                    notes += $"{line}\n";
                }
            }

            var outputDirectory = releaseNotesOptions.CreateInRoot ? Build.SolutionDirectory : Build.ProjectDirectory;

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            File.WriteAllText(Path.Combine(outputDirectory, releaseNotesOptions.FileName), notes.Trim());
        }

        internal IEnumerable<string> GetCommitDates(DateTime fromDate)
        {
            var args = $@"log --no-merges --since=""{fromDate.Year}-{fromDate.Month}-{fromDate.Day} 00:00:00"" --format="" % cd"" --date=short";
            return GetProcessOutput(args).Distinct();
        }

        internal IEnumerable<string> GetCommitMessages(string date)
        {
            var args = $@"git log --no-merges --format="" * % s"" --since=""{date} 00:00:00"" --until=""{date} 24:00:00""";
            return GetProcessOutput(args);
        }

        internal string GetBranchName()
        {
            var branches = GetProcessOutput("branch");
            var branch = branches.FirstOrDefault(l => l.Contains("*"));
            if(!string.IsNullOrWhiteSpace(branch))
            {
                branch = Regex.Replace(branch, "*", "").Trim();
            }
            return branch;
        }

        private string[] GetProcessOutput(string args)
        {
            var pci = new ProcessStartInfo("git", args)
            {
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            };
            using (var process = Process.Start(pci))
            {
                process.WaitForExit();
                string output = null;
                using (var reader = process.StandardOutput)
                {
                    output = reader.ReadToEnd();
                }
                process.Dispose();
                return output.Split(
                            new[] { Environment.NewLine },
                            StringSplitOptions.None
                        );
            }
        }
    }
}
