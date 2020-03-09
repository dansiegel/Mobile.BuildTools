namespace Mobile.BuildTools.Handlers
{
    internal class UriValueHandler : DefaultValueHandler
    {
        public UriValueHandler()
            : base("new System.Uri(\"{0}\", System.UriKind.Absolute)")
        {
        }
    }
}
