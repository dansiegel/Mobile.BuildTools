using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FormsDemo.Services;
using FormsDemo.Views;

namespace FormsDemo
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage();
        }
    }
}
