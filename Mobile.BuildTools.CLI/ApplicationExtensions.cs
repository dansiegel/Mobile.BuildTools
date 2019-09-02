using System;
using System.Collections.Generic;
using System.Text;
using McMaster.Extensions.CommandLineUtils;
using Mobile.BuildTools.Commands;

namespace Mobile.BuildTools
{
    public static class ApplicationExtensions
    {
        public static CommandLineApplication Register(this CommandLineApplication app, IRegisterable registerable)
        {
            registerable.Register(app);
            return app;
        }
    }
}
