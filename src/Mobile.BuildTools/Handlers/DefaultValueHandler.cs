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

        public string Format(string args) => string.Format(_format, args);
    }
}
