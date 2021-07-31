using System.Linq;

namespace Mobile.BuildTools.Handlers
{
    internal class DefaultValueHandler : IValueHandler
    {
        public DefaultValueHandler()
            : this(null)
        {
        }

        public DefaultValueHandler(string format)
        {
            _format = string.IsNullOrWhiteSpace(format) ? "{0}" : format;
        }

        private string _format { get; }

        public string Format(string args, bool safeOutput)
        {
            if (safeOutput)
                args = string.Join(string.Empty, args.Select(c => "*"));

            return string.Format(_format, args);
        }
    }
}
