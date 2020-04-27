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

        protected override void ExecuteInternal()
        {
            var releaseNotesOptions = Build.Configuration.ReleaseNotes;

            if (releaseNotesOptions.Disable) return;

            var currentHash = GetGitHash();
            var generatedMarkerDirectory = Path.Combine(Build.SolutionDirectory, ".vs");

            if (!Directory.Exists(generatedMarkerDirectory))
                Directory.CreateDirectory(generatedMarkerDirectory);

            var generatedMarkerFilePath = Path.Combine(generatedMarkerDirectory, $"generated-releasenotes.{currentHash}");

            var outputDirectory = releaseNotesOptions.CreateInRoot ? Build.SolutionDirectory : Build.ProjectDirectory;

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            var releaseNotesFilePath = Path.Combine(outputDirectory, releaseNotesOptions.FileName);

            if (File.Exists(releaseNotesFilePath) && File.Exists(generatedMarkerFilePath))
            {
                Log.LogMessage("Current Release Notes file has already been generated.");
                return;
            }

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

                var lines = GetCommitMessages(date);
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

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

            File.WriteAllText(releaseNotesFilePath, notes.Trim());
            File.WriteAllText(generatedMarkerFilePath, @"99 little bugs in the code,
99 bugs in the code,
1 bug fixed... compile again,
100 little bugs in the code.");
        }

        internal IEnumerable<string> GetCommitDates(DateTime fromDate)
        {
            var args = $@"log --no-merges --since=""{fromDate.Year}-{fromDate.Month}-{fromDate.Day} 00:00:00"" --format="" % cd"" --date=short";
            var output = GetProcessOutput(args);

            var lines = new List<string>();
            foreach(var line in output)
            {
                var sanitized = Regex.Replace(line, @"\\n", "\n");
                lines.AddRange(sanitized.Split('\n').Select(x => x.Trim()));
            }

            return lines.Distinct();
        }

        internal IEnumerable<string> GetCommitMessages(string date)
        {
            var args = $@"log --no-merges --format="" * % s"" --since=""{date} 00:00:00"" --until=""{date} 24:00:00""";
            return GetProcessOutput(args);
        }

        internal string GetBranchName()
        {
            var branches = GetProcessOutput("branch");
            var branch = branches.FirstOrDefault(l => l.Contains("*"));
            if(!string.IsNullOrWhiteSpace(branch))
            {
                branch = Regex.Replace(branch, Regex.Escape("*"), string.Empty).Trim();
            }
            return branch;
        }

        private string GetGitHash()
        {
            var outputs = GetProcessOutput("log -1 --format=\"%H\"");
            return outputs.FirstOrDefault()?.Trim();
        }

        private string[] GetProcessOutput(string args)
        {
            var pci = new ProcessStartInfo("git", args)
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,

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
