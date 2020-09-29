using System;
using BuildToolsSample.Views;
using Prism.Ioc;
using Xamarin.Forms;

namespace BuildToolsSample
{
    public partial class App
    {
        public App()
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            var result = await NavigationService.NavigateAsync("MainPage/NavigationPage/HomePage");

            if(!result.Success)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<AppConfigPage>();
            containerRegistry.RegisterForNavigation<DocsPage>();
            containerRegistry.RegisterForNavigation<HomePage>();
        }
    }
}
