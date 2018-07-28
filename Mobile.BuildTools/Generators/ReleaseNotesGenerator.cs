using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Generators
{
    public class ReleaseNotesGenerator : IGenerator
    {
        public string OutputDirectory { get; set; }
        public DateTime FromDate { get; set; }
        public int MaxCommit { get; set; }
        public int CharacterLimit { get; set; }
        public ILog Log { get; set; }
        public bool? DebugOutput { get; set; }

        public void Execute()
        {
            var branchName = GetBranchName();
            if (string.IsNullOrWhiteSpace(branchName))
            {
                Log.LogMessage("It appears the git repo is in a disconnected state, unable to determine the current branch name");
                return;
            }

            //"git log --no-merges --since="$TODAY 00:00:00" --format=" % cd" --date=short";
            var dates = GetCommitDates();
            var notes = string.Empty;
            var characterLimitExceeded = false;
            var commits = 0;
            foreach (var date in dates)
            {
                // 2018-07-25
                //"git log --no-merges --format=" * % s" --since="2018-07-25 00:00:00" --until="2018-07-25 24:00:00""
                if (commits > MaxCommit || characterLimitExceeded)
                {
                    Log.LogMessage("Maximimum limits reaches. Truncating Release Notes.");
                    break;
                }
                    
                foreach (var line in GetCommitMessages(date))
                {
                    if (MaxCommit > 0 && commits++ > MaxCommit)
                        break;
                    if(CharacterLimit > 0 && line.Length + notes.Length > CharacterLimit)
                    {
                        characterLimitExceeded = true;
                        break;
                    }
                    notes += $"{line}\n";
                }
            }

            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);

            File.WriteAllText(Path.Combine(OutputDirectory, "releasenotes.md"), notes.Trim());
        }

        internal IEnumerable<string> GetCommitDates()
        {
            var args = $@"log --no-merges --since=""{FromDate.Year}-{FromDate.Month}-{FromDate.Day} 00:00:00"" --format="" % cd"" --date=short";
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
