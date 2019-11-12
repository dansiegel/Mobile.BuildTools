using System;
using System.Collections.Generic;
using System.Linq;
using Mobile.BuildTools.Commands;
using McMaster.Extensions.CommandLineUtils;
using System.Diagnostics;

namespace Mobile.BuildTools
{
    class Program : CliBase
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                Name = Settings.CommandName,
                FullName = "Mobile BuildTools CLI Tool"
            };
            app.HelpOption();

            // register commands
            new PackAndSignCoreFoundationCommand().Register(app);

            // no command
            var showHelp = false;
            app.OnExecute(() => {
                showHelp = true;
                Console.WriteLine(app.GetHelpText());
            });

            // execute command line options and report any errors
            var stopwatch = Stopwatch.StartNew();
            using (new AnsiTerminal(Settings.UseAnsiConsole))
            {
                try
                {
                    try
                    {
                        app.Execute(args);
                    }
                    catch (CommandParsingException e)
                    {
                        LogError(e.Message);
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                    if (Settings.HasErrors)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"FAILED: {Settings.ErrorCount:N0} errors encountered");
                        Settings.ShowErrors();
                        return -1;
                    }
                    return 0;
                }
                finally
                {
                    if (!showHelp)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Done (finished: {DateTime.Now}; duration: {stopwatch.Elapsed:c})");
                    }
                }
            }
        }

        public override void Register(CommandLineApplication app)
        {
            throw new NotImplementedException();
        }
    }
}
