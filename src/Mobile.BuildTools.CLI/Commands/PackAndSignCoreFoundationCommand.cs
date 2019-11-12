using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;

namespace Mobile.BuildTools.Commands
{
    internal class PackAndSignCoreFoundationCommand : CliBase
    {
        public override void Register(CommandLineApplication app)
        {
            // add 'build' command
            app.Command("build", cmd => {
                cmd.HelpOption();
                cmd.Description = "Build LambdaSharp module";
                });
        }
    }
}
