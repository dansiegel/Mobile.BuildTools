using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mobile.BuildTools.Tasks
{
    public class BuildEnvironmentDumpTask : Microsoft.Build.Utilities.Task
    {
        public string ProjectDirectory { get; set; }

        public override bool Execute()
        {
            var variables = Utils.EnvironmentAnalyzer.GatherEnvironmentVariables(ProjectDirectory, true);

            string message = null;
            if(variables.Any())
            {
                message = $"Found {variables.Count} Environment Variables\n";
                foreach(var variable in variables)
                {
                    message += $"    {variable.Key}: {variable.Value}\n";
                }
            }
            else
            {
                message = "There doesn't seem to be any Environment Variables... something is VERY VERY Wrong...";
            }

            Log.LogMessage(Microsoft.Build.Framework.MessageImportance.High, message);

            return true;
        }
    }
}
