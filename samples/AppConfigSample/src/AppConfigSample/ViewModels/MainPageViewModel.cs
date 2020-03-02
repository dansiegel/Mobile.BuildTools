using System.Collections.Generic;
using Mobile.BuildTools.Configuration;
using Prism.Commands;
using Prism.Mvvm;

namespace AppConfigSample.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private IConfigurationManager _configurationManager { get; }

        public MainPageViewModel(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
            SetEnvironmentCommand = new DelegateCommand<string>(OnSetEnvironmentCommandExecuted, x => !string.IsNullOrEmpty(x));
            Update();
        }

        private string _foo;
        public string Foo
        {
            get => _foo;
            set => SetProperty(ref _foo, value);
        }

        private string _bar;
        public string Bar
        {
            get => _bar;
            set => SetProperty(ref _bar, value);
        }

        private string _test;
        public string Test
        {
            get => _test;
            set => SetProperty(ref _test, value);
        }

        public IEnumerable<string> Environments => new[]
        {
            "Default",
            "Debug",
            "Foo",
            "Release"
        };

        public DelegateCommand<string> SetEnvironmentCommand { get; }

        private void OnSetEnvironmentCommandExecuted(string environment)
        {
            if(environment.Equals("Default"))
            {
                _configurationManager.Reset();
            }
            else
            {
                _configurationManager.Transform(environment);
            }

            Update();
        }

        private void Update()
        {
            Foo = _configurationManager.AppSettings["foo"];
            Bar = _configurationManager.AppSettings["bar"];
            Test = _configurationManager.ConnectionStrings["test"].ConnectionString;
        }
    }
}
