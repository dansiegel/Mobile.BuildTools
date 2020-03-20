using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mobile.BuildTools.Configuration;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppConfigSample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabB : ContentPage
    {
        public TabB()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var environments = new List<string> { "Default" };
            environments.AddRange(ConfigurationManager.Environments);
            environmentsSelector.ItemsSource = environments;
            environmentsSelector.SelectedIndex = 0;
            UpdateUI();
        }

        private void OnUpdateClicked(object sender, EventArgs args)
        {
            var environment = (string)environmentsSelector.SelectedItem;
            if(string.IsNullOrEmpty(environment))
            {
                DisplayAlert("Error", "You must select an enviornment", "Ok");
                return;
            }

            if (environment.Equals("Default"))
            {
                ConfigurationManager.Reset();
            }
            else
            {
                ConfigurationManager.Transform(environment);
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            try
            {
                fooLabel.Text = $"Foo: {ConfigurationManager.AppSettings["foo"]}";
                barLabel.Text = $"Bar {ConfigurationManager.AppSettings["bar"]}";
                testLabel.Text = $"Test: {ConfigurationManager.ConnectionStrings["test"].ConnectionString}";
                environmentLabel.Text = $"Environment: {ConfigurationManager.AppSettings["Environment"]}";
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
                System.Diagnostics.Debugger.Break();
            }
        }
    }
}