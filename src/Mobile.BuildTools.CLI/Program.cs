using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using Mobile.BuildTools.Commands;

namespace Mobile.BuildTools
{
    public class Program : CliBase
    {
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication()
            {
                Name = Settings.CommandName,
                FullName = $"Mobile.BuildTools CLI (v{Version})",
                Description = "Project Home: https://github.com/dansiegel/Mobile.BuildTools",
                UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.Throw
            };
            app.HelpOption();
        }
    }
}
