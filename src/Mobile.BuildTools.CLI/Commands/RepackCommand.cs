using System;
using McMaster.Extensions.CommandLineUtils;

namespace Mobile.BuildTools.Commands
{
    [Command(
        Description = "Repack will explode your IPA, APK, or AAB, replace assets and then rebundle the app package. Note that IPA are ONLY supported on Mac.",
        Name = CommandName)]
    public class RepackCommand 
    {
        public const string CommandName = "repack";

        [Argument(0, Name = "--app-path", ShowInHelpText = true)]
        public string AppPath { get; set; }

        private void OnExecute()
        {
            Console.WriteLine("Hello World");
        }
    }



    public abstract class ACliCommand : CliBase, IRegisterable
    {
        protected abstract void OnExecute();
    }

    public interface IRegisterable
    {
    }
}
