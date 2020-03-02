using System;
using Prism.Ioc;
using AppConfigSample.ViewModels;
using AppConfigSample.Views;
using Xamarin.Forms;
using Mobile.BuildTools.Configuration;

namespace AppConfigSample
{
    public partial class App
    {
        public App()
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            var result = await NavigationService.NavigateAsync("MainPage");

            if(!result.Success)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterInstance<IConfigurationManager>(ConfigurationManager.Current);
        }
    }
}
