namespace Mobile.BuildTools.Configuration
{
    public interface IConfigurationManager
    {
        INameValueCollection AppSettings { get; }
        ConnectionStringCollection ConnectionStrings { get; }

        bool EnvironmentExists(string name);
        IReadOnlyList<string> Environments { get; }
        event EventHandler SettingsChanged;
        void Reset();
        void Transform(string name);
    }
}
